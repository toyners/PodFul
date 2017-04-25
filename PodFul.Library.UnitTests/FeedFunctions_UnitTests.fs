namespace PodFul.Library.UnitTests

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
        let result = FeedFunctions.CleanText dirtyText
        Assert.AreEqual(result, "a b")

    [<Test>]
    [<TestCase("a  b")>]
    [<TestCase("a   b")>]
    member public this.``Clean text of multiple spaces``(dirtyText : string) =
        let result = FeedFunctions.CleanText dirtyText
        Assert.AreEqual(result, "a b")

    [<Test>]
    [<TestCase("<p>")>]
    [<TestCase("<P>")>]
    [<TestCase("</p>")>]
    [<TestCase("</P>")>]
    member public this.``Clean text of tags``(dirtyText : string) =
        let result = FeedFunctions.CleanText dirtyText
        Assert.AreEqual(result, String.Empty)

    [<Test>]
    [<TestCase("&#8217;", "'")>]
    [<TestCase("&#8230;", "...")>]
    member public this.``Clean text of known XML character codes``(dirtyText : string, cleanText : string) =
        let result = FeedFunctions.CleanText dirtyText
        Assert.AreEqual(result, cleanText)

    [<Test>]
    [<TestCase("&#124;", "")>]
    [<TestCase("&#9999;", "")>]
    member public this.``Clean text of unknown XML character codes``(dirtyText : string, cleanText : string) =
        let result = FeedFunctions.CleanText dirtyText
        Assert.AreEqual(result, cleanText)

    [<Test>]
    [<TestCase("<p>", "")>]
    [<TestCase("<P>", "")>]
    [<TestCase("</p>", "")>]
    [<TestCase("</P>", "")>]
    [<TestCase("<span key=\"value\">text</span>", "text")>]
    member public this.``Clean text of XML tags``(dirtyText : string, cleanText : string) =
        let result = FeedFunctions.CleanText dirtyText
        Assert.AreEqual(result, cleanText)
 
    [<Test>]
    member public this.``Clean text of leading and trailing whitespace``() =
        let result = FeedFunctions.CleanText " abc "
        Assert.AreEqual(result, "abc")

    [<Test>]
    member public this.``Create feed with bad url returns exception``() =
        let guid = Guid.NewGuid()
        let badURL = "http://" + guid.ToString() + ".com"
        let directory = @"C:\directory\"

        let mutable testSuccessful = false
        try 
            FeedFunctions.CreateFeed badURL null directory "" System.Threading.CancellationToken.None |> ignore
        with
        | _ as e ->
            let r = e :? System.Net.WebException
            Assert.IsNotNull(r)
            
            testSuccessful <- true

        Assert.AreEqual(true, testSuccessful)
