using Newtonsoft.Json;

namespace ConsoleApp1;

public class SightingsRepository {

    private List<Sighting> sightings;

    //TODO make this read in all the json files 
    public SightingsRepository(){
        var projPath = ElementExtensions.TryGetSolutionDirectoryInfo();
        Console.WriteLine(projPath.FullName);
        var dataPath = Path.Combine(projPath.Parent.FullName,"data");
        string text = File.ReadAllText(Path.Combine(dataPath,"May-2024.json"));
        // Console.WriteLine(text);
        sightings = JsonConvert.DeserializeObject<List<Sighting>>(text);
    }
    
    public List<Sighting> GetSightings(String month, int year){
        return sightings;
    }
}