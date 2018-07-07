namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.WPF

/// <summary>
/// Tests the different values that the datetime to string converter can be passed. Creates user friendly strings. 
/// </summary>
type GeneralDateTimeConverter_UnitTests() = 
     
     [<Test>]
     member public this.``Value is null so empty string is returned.``() =
        let converter = new GeneralDateTimeConverter()
        Assert.AreEqual(String.Empty, converter.Convert(null, null, null, null))

     [<Test>]
     member public this.``Value is not of datetime type so empty string is returned.``() =
        let converter = new GeneralDateTimeConverter()
        Assert.AreEqual(String.Empty, converter.Convert(new Object(), null, null, null))

     [<Test>]
     member public this.``Value is DateTime MinValue so Never is returned.``() =
        let converter = new GeneralDateTimeConverter()
        Assert.AreEqual("Never", converter.Convert(DateTime.MinValue, null, null, null))

     [<Test>]
     member public this.``Value is an hour ago so string starts with Today.``() =
        let converter = new GeneralDateTimeConverter()
        let value = DateTime.Now - TimeSpan.FromHours(1.0)
        let resultTime = value.ToString("HH:mm:ss");
        Assert.AreEqual(("Today at " + resultTime), converter.Convert(value, null, null, null))

     [<Test>]
     member public this.``Value is a day ago so string starts with Yesterday.``() =
        let converter = new GeneralDateTimeConverter()
        let value = DateTime.Now - TimeSpan.FromDays(1.0)
        let resultTime = value.ToString("HH:mm:ss");
        Assert.AreEqual(("Yesterday at " + resultTime), converter.Convert(value, null, null, null))

     [<Test>]
     member public this.``Value is last night so string starts with Yesterday ``() =
        let converter = new GeneralDateTimeConverter()
        let yesterday = DateTime.Now - TimeSpan.FromDays(1.0)
        let value = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59)
        let resultTime = value.ToString("HH:mm:ss");
        Assert.AreEqual(("Yesterday at " + resultTime), converter.Convert(value, null, null, null))

     [<Test>]
     [<TestCase(2)>]
     [<TestCase(3)>]
     [<TestCase(4)>]
     [<TestCase(5)>]
     [<TestCase(6)>]
     [<TestCase(7)>]
     member public this.``Value is up to a week ago so string starts with Day name.``(dayCount : float) =
        let converter = new GeneralDateTimeConverter()
        let value = DateTime.Now - TimeSpan.FromDays(dayCount)
        let dayName = value.ToString("dddd")
        let resultTime = value.ToString("HH:mm:ss");
        Assert.AreEqual((dayName + " at " + resultTime), converter.Convert(value, null, null, null))

     [<Test>]
     [<TestCase(8)>]
     [<TestCase(999)>]
     member public this.``Value is over a week ago so string starts with Date.``(dayCount : float) =
        let converter = new GeneralDateTimeConverter()
        let value = DateTime.Now - TimeSpan.FromDays(dayCount)
        let resultDateTime = value.ToString("dd-MMM-yyyy HH:mm:ss");
        Assert.AreEqual(resultDateTime, converter.Convert(value, null, null, null))
