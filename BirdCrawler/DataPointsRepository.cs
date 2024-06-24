using Newtonsoft.Json;

namespace ConsoleApp1;

public class DataPointsRepository {
    public void Store(List<SightingDataPoint> dataPoints){
        var path = ElementExtensions.TryGetSolutionDirectoryInfo().Parent.FullName;
        var datapoints = JsonConvert.SerializeObject(dataPoints, Formatting.Indented);
        
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, $"datapoints.json")))
        { 
            outputFile.WriteLine(datapoints);
        }
    }
}