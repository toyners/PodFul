namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection

type FeedFunctions_UnitTests() = 

    [<Test>]
    [<TestCase("\r\n")>]
    [<TestCase("\n")>]
    member public this.``Clean text of line breaks``(dirtyText : string) =
        FeedFunctions.CleanText dirtyText |> should equal " "

    [<Test>]
    [<TestCase("  ")>]
    [<TestCase("   ")>]
    member public this.``Clean text of multiple spaces``(dirtyText : string) =
        FeedFunctions.CleanText dirtyText |> should equal " "

    [<Test>]
    [<TestCase("<p>")>]
    [<TestCase("<P>")>]
    [<TestCase("</p>")>]
    [<TestCase("</P>")>]
    member public this.``Clean text of tags``(dirtyText : string) =
        FeedFunctions.CleanText dirtyText |> should equal String.Empty

    [<Test>]
    [<TestCase("&#8217;", "'")>]
    [<TestCase("&#124;", "|")>]
    [<TestCase("&#8230;", "...")>]
    member public this.``Clean text of XML character codes``(dirtyText : string, cleanText : string) =
        FeedFunctions.CleanText dirtyText |> should equal cleanText
