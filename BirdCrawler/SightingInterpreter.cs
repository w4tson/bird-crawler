using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace ConsoleApp1;

public record OpenAIMessage(String role, String content);
public record OpenAIRequest(String model, List<OpenAIMessage> messages, float temperature);
public record Choice(OpenAIMessage message);
public record OpenAIResponse(List<Choice> choices);

public record OpenAIDataPoint(String species, String location, int count);
public record SightingDataPoint(String species, String location, int count, DateTime date);

public class SightingInterpreter{
    HttpClient client;

    private String prompt =
        """
        Slimbridge is bird sanctuary in England. Within it are a number of areas. 
        
        These areas are Big Pen, Decoy Hide, Eider Pen, Estuary tower, Finger Hide, Garnett Hide, Hogarth Hide, Kingfisher Hide, Kirk Hide, 
        Knott Hide, Martin Smith hide, Middle Point, Peng Hide, Rushy Hide, Shepherd’s Hut, Sloane Tower, Smith Hide, South Lake, Stephen Kirk hide, 
        Summer Walkway, Tack Piece, Van de Bovenkamp Hide, Willow Hide and Zeiss Hide.

        Take the following entry from a bird spotting website and count the number of species of birds in the text 
        together with the location in Slimbridge where they were found with count for each. 
        Output the answer in json format only. No extra text. The json format should be a list of objects containing 
        3 properties: "species", "location" and "count". An example might be [{ "Species" : "Kingfisher", "location": "South Lake", count: 2 }].
        If there is no location then just use and empty string, if there is no sighting just return an empty array: []. 
        
        
        """;
    public SightingInterpreter(){
        client = new HttpClient();
        client.BaseAddress = new Uri("https://api.openai.com");

        // Add an Accept header for JSON format.
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        var api_key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        client.DefaultRequestHeaders.Add("Authorization",$"Bearer {api_key}");
    }

    public List<SightingDataPoint> GetDataPoints(String sightingDescription){
        return [new("Avocet", "Rushy Hide", 30, DateTime.Now)];
    }
    
    public List<SightingDataPoint>? GetDataPointsReal(Sighting sighting){
        // var sighting = "48 Avocets with at least 13 nests on the Rushy and 71 Avocets with 14 active nests on South Lake. A Garganey reported from Zeiss hide. (Dot Jones/Ian Hull)";
        
        var response = CallOpenAI(prompt+$"\"{sighting.Text}\"");
        Console.WriteLine(response);
        List<OpenAIDataPoint> openAiDataPoints;
        try
        {
            openAiDataPoints = JsonConvert.DeserializeObject<List<OpenAIDataPoint>>(response);
        }
        catch (Exception e)
        {
            Console.WriteLine($"sighting failed for {sighting.Text}");
            openAiDataPoints =[];
        }
        
        return openAiDataPoints
            .Select(d => new SightingDataPoint(d.species, d.location, d.count, sighting.Date))
            .ToList();
    }

    private  String? CallOpenAI(String request){
        var openAiRequest = new OpenAIRequest("gpt-4", new List<OpenAIMessage>(){ new("user", request) }, 0.7f);
        var body = JsonContent.Create(openAiRequest);

        // var api_key = Environment.GetEnvironmentVariable("OPENAI_KEY");
        HttpResponseMessage response = client.PostAsync("/v1/chat/completions", body).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
        if (response.IsSuccessStatusCode)
        {
            var openaiResponse = response.Content.ReadFromJsonAsync<OpenAIResponse>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
            // Console.WriteLine(openaiResponse.choices.First().message);
            return openaiResponse.choices.First().message.content;
        }
        else
        {
            Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            return null;
        }
    }
}