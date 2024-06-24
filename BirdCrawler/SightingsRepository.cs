using Newtonsoft.Json;

namespace ConsoleApp1;

public class SightingsRepository {

    private List<Sighting> sightings;

    //TODO make this read in all the json files 
    public SightingsRepository(){
        var projPath = ElementExtensions.TryGetSolutionDirectoryInfo();
        var dataPath = Path.Combine(projPath.Parent.FullName,"data");
        
        // read in text from json file as string
        string text = File.ReadAllText(Path.Combine(dataPath,"January-2024.json"));
        
        // marshall into a list of Sighting records
        sightings = JsonConvert.DeserializeObject<List<Sighting>>(text);
    }
    
    //TODO actually filter these 
    public List<Sighting> GetSightings(String month, int year){
        return sightings;
    }
}