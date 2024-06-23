using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ConsoleApp1;

namespace ConsoleApp1.Tests;

[TestClass]
[TestSubject(typeof(SlimbridgeCrawler))]
public class SlimbridgeCrawlerTest{

    [TestMethod]
    public void testy(){
        var date = new SlimbridgeCrawler().toDate(@"https://theglosterbirder.co.uk/2024/05/13/sunday-12th-august-2024/");
        Assert.AreEqual(new DateTime(2024,08,12),date);
    }


    [TestMethod]
    public void testRecordToJson(){
        var sighting = new Sighting("href", "some location", "some text",new DateTime(2024, 05, 01));
        var json = JsonConvert.SerializeObject(new List<Sighting>() {sighting,sighting}, Formatting.Indented);
        Console.WriteLine(json);
    }


    [TestMethod]
    public void testReplaceFirst(){
        var r = "foo bar foo".ReplaceFirst("foo", "fizz");
        Console.WriteLine(r);
    }



    [TestMethod]
    public void TestJsonDeserialization(){
        var projPath = ConsoleApp1.ElementExtensions.TryGetSolutionDirectoryInfo();
        Console.WriteLine(projPath.FullName);
        var dataPath = Path.Combine(projPath.Parent.FullName,"data");
        string text = File.ReadAllText(Path.Combine(dataPath,"April-2024.json"));
        // Console.WriteLine(text);
        var result = JsonConvert.DeserializeObject<List<Sighting>>(text);
        foreach (var sighting in result)
        {
            // Console.WriteLine(sighting);
        }
    }
}