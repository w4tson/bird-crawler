using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ConsoleApp1.Tests;

[TestClass]
[TestSubject(typeof(BirdCrawler))]
public class BirdCrawlerTest{

    [TestMethod]
    public void testy(){
        var date = new BirdCrawler().toDate(@"https://theglosterbirder.co.uk/2024/05/13/sunday-12th-august-2024/");
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
        var projPath = ElementExtensions.TryGetSolutionDirectoryInfo();
        Console.WriteLine(projPath.FullName);
        var dataPath = Path.Combine(projPath.Parent.FullName,"data");
        string text = File.ReadAllText(Path.Combine(dataPath,"April-2024.json"));
        var result = JsonConvert.DeserializeObject<List<Sighting>>(text);
        foreach (var sighting in result)
        {
            Console.WriteLine(sighting);
        }
    }


    [TestMethod]
    public void TestOpenAI(){
        Console.WriteLine("start");
        var i = new SightingInterpreter();
        // don't call this every time cos it uses up money!
        //i.GetDataPoints("48 Avocets with at least 13 nests on the Rushy and 71 Avocets with 14 active nests on South Lake. A Garganey reported from Zeiss hide. (Dot Jones/Ian Hull)");
            
    }
}