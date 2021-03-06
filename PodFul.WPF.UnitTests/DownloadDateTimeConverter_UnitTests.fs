﻿namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.WPF

type DownloadDateTimeConverter_UnitTests() = 

    [<Test>]
    member public this.``Value is null so empty string is returned.``() =
        let converter = new DownloadDateTimeConverter()
        Assert.AreEqual(String.Empty, converter.Convert(null, null, null, null))

    [<Test>]
    member public this.``Value is not of datetime type so empty string is returned.``() =
        let converter = new DownloadDateTimeConverter()
        Assert.AreEqual(String.Empty, converter.Convert(new Object(), null, null, null))

    [<Test>]
    member public this.``Value is DateTime MinValue so No download string is returned.``() =
        let converter = new DownloadDateTimeConverter()
        Assert.AreEqual("No download", converter.Convert((DateTime.MinValue, null, null, null)))

    [<Test>]
    [<TestCase(2006, 11, 15, 23, 59, 59, "15-Nov-2006 23:59:59")>]
    [<TestCase(2006, 11, 5, 2, 6, 7, "5-Nov-2006 2:06:07")>]
    member public this.``Value is valid DateTime so date time string is returned.``(year, month, day, hour, minute, second, expected : string) =
        let converter = new DownloadDateTimeConverter()
        let value = new DateTime(year, month, day, hour, minute, second)
        Assert.AreEqual(expected, converter.Convert(value, null, null, null))
