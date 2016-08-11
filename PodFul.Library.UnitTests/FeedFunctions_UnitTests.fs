namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection
open System.Xml.Linq

type FeedFunctions_UnitTests() = 

    [<Test>]
    [<TestCase("a\r\nb")>]
    [<TestCase("a\nb")>]
    member public this.``Clean text of line breaks``(dirtyText : string) =
        FeedFunctions.CleanText dirtyText |> should equal "a b"

    [<Test>]
    [<TestCase("a  b")>]
    [<TestCase("a   b")>]
    member public this.``Clean text of multiple spaces``(dirtyText : string) =
        FeedFunctions.CleanText dirtyText |> should equal "a b"

    [<Test>]
    [<TestCase("<p>")>]
    [<TestCase("<P>")>]
    [<TestCase("</p>")>]
    [<TestCase("</P>")>]
    member public this.``Clean text of tags``(dirtyText : string) =
        FeedFunctions.CleanText dirtyText |> should equal String.Empty

    [<Test>]
    [<TestCase("&#8217;", "'")>]
    
    [<TestCase("&#8230;", "...")>]
    member public this.``Clean text of known XML character codes``(dirtyText : string, cleanText : string) =
        FeedFunctions.CleanText dirtyText |> should equal cleanText

    [<Test>]
    [<TestCase("&#124;", "")>]
    [<TestCase("&#9999;", "")>]
    member public this.``Clean text of unknown XML character codes``(dirtyText : string, cleanText : string) =
        FeedFunctions.CleanText dirtyText |> should equal cleanText

    [<Test>]
    [<TestCase("<p>", "")>]
    [<TestCase("<P>", "")>]
    [<TestCase("</p>", "")>]
    [<TestCase("</P>", "")>]
    [<TestCase("<span key=\"value\">text</span>", "text")>]
    member public this.``Clean text of XML tags``(dirtyText : string, cleanText : string) =
        FeedFunctions.CleanText dirtyText |> should equal cleanText
         
    [<Test>]
    member public this.``Clean text of leading and trailing whitespace``() =
        FeedFunctions.CleanText " abc " |> should equal "abc"

    [<Test>]
    member public this.``Create feed with bad url returns retry exception``() =
        let guid = Guid.NewGuid()
        let badURL = "http://" + guid.ToString() + ".com"
        let directory = @"C:\directory\"
        let ex = (fun() -> FeedFunctions.CreateFeed badURL directory null |> ignore)
        ex |> should throw typeof<FeedFunctions.RetryException>