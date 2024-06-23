using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PuppeteerSharp;

namespace ConsoleApp1;

public record Sighting(string Href, string location, string Text, DateTime Date);


class BirdCrawler{
    private IPage page;
    
    private Regex hrefDateRegex = new Regex("""^https:\/\/theglosterbirder\.co\.uk\/\d{4}\/\d{2}\/\d{2}\/\w+-(\d+)\w{2}-(\w+)-(\d{4})\/$""");
    
    static void Main(string[] args){
        // Run().GetAwaiter().GetResult();
        Task.Run(() => RunCrawl()).Wait();
    }

    static async Task RunCrawl(){
        var crawler = new BirdCrawler();
        await crawler.openGlosterBirder();
        
        //TODO september 2023 inconsistent url!
        
        var res = await crawler.GetEntriesForMonth("May", 2023);
        // await crawler.GetEntriesForMonth("February", 2024);
        // await crawler.GetEntriesForMonth("March", 2024);
        //var sightings = await sc.GetSightingsForHref("https://theglosterbirder.co.uk/2024/05/13/sunday-12th-may-2024/");
        //var json = JsonConvert.SerializeObject(sightings, Formatting.Indented);
        
        Console.WriteLine($"Finished {res}");
    }

    public async Task openGlosterBirder() {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            DefaultViewport = new ViewPortOptions { Width = 1200, Height = 800 }
        });

        page = await browser.NewPageAsync();
        await page.GoToAsync("https://theglosterbirder.co.uk/");
    }

    public async Task<int> GetEntriesForMonth(String month, int year)
    {
        // Find hyper link on the right hand side by name
        var monthLink = await page.XPathAsync($"//a[text()='{month} {year}']");

        // Click it
        await monthLink.First().ClickAsync();

        // Wait for new page to load
        await page.WaitForNavigationAsync(new NavigationOptions(){ Timeout = 5000 });

        // Scroll down to load more content (need to sleep wait ☹️)
        await page.Mouse.WheelAsync(0, 60000);
        Thread.Sleep(2000);
        await page.Mouse.WheelAsync(0, 20000);
        Thread.Sleep(1000);

        // find all Heading 2 elements with a css class of "entry-title"
        var titles = await page.QuerySelectorAllAsync("h2.entry-title");

        var hrefs = titles.Select(async title => {
                // using an xpath selector get the parent element and search down from there for all <a> tags
                var continueReadingLink = await title.XPathAsync("..//a");

                // extract the href
                var href = await continueReadingLink.First().GetHref();
                return href;
            })
            .Select(e => e.Result)
            .ToList();

        var sightings = new List<Sighting>();
        
        Console.WriteLine("Done. \n\n");
        
        foreach (var href in hrefs)
        {
            var entriesForDay = await GetSightingsForHref(href);
            sightings.AddRange(entriesForDay);
        }
        var sightingsJson = JsonConvert.SerializeObject(sightings, Formatting.Indented);

        var path = ElementExtensions.TryGetSolutionDirectoryInfo().Parent.FullName;
        Console.WriteLine($"path = {path}");
        
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, $"{month}-{year}.json")))
        { 
            outputFile.WriteLine(sightingsJson);
        }
        
        Console.WriteLine(sightingsJson);


        // var result = sightings
        //     .GroupBy(s => s.Date)
        //     .ToDictionary(x => x.Key, x => x.ToList());
        //
        
        // foreach (var key in result.Keys)
        // {
        //     Console.WriteLine(key);
        //     foreach (var sighting in result[key])
        //     {
        //         Console.WriteLine("\t"+sighting.Text);
        //     }
        // }
        return 0;
    }

    public async Task<List<Sighting>> GetSightingsForHref(String href){
        await page.GoToAsync(href);
        var allSightingLocations = await page.XPathAsync("//p/strong");
        
        Console.WriteLine("processing "+ href);
        
        return allSightingLocations.Select(async sightingLocation => {
                var location = (await sightingLocation.GetInnerTextAsync()).Trim();
                
                var sightingElement = await sightingLocation.XPathAsync("..");
                var slimbridgeEntryText = await sightingElement.First().GetInnerTextAsync();
                var text = slimbridgeEntryText.ReplaceFirst(location,"");
                
                return new Sighting(href, location, text.Trim(), toDate(href));
            })
            .Select(e => e.Result)
            .ToList();
    }

    public DateTime toDate(String href){
        var matches  = hrefDateRegex.Matches(href);
        var match = matches.First();
        var day = match.Groups[1].Value;
        var month = match.Groups[2].Value;
        var year = match.Groups[3].Value;

        return DateTime.ParseExact($"{day.PadLeft(2,'0')}/{month}/{year}", "dd/MMMM/yyyy", null);
    }
}