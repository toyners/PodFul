namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.WPF

type TextTruncator_UnitTests() =

    [<Test>]
    member public this.``Value is not of string type so empty string is returned.``() =
        let converter = new TextTruncator()
        Assert.AreEqual(String.Empty, converter.Convert(new Object(), null, null, null))

    [<Test>]
    member public this.``Text length is below maximum character limit so full text is returned.``() =
        let converter = new TextTruncator()
        Assert.AreEqual("The red fox", converter.Convert("The red fox", null, "17", null))

    [<Test>]
    member public this.``Text length is at maximum character limit so full text is returned.``() =
        let converter = new TextTruncator()
        Assert.AreEqual("The quick red fox", converter.Convert("The quick red fox", null, "17", null))

    [<Test>]
    member public this.``Text length is above maximum character limit so truncated text is returned.``() =
        let converter = new TextTruncator()
        Assert.AreEqual("The quick red ...", converter.Convert("The quick red fox jumped over the lazy brown dog", null, "17", null))

    [<Test>]
    member public this.``Word is split by maximum character limit so is not included in truncated text.``() =
        let converter = new TextTruncator()
        Assert.AreEqual("The quick ...", converter.Convert("The quick red fox jumped over the lazy brown dog", null, "16", null))

    [<Test>]
    member public this.``First Word is too long so full text is returned.``() =
        let converter = new TextTruncator()
        let text = "abcdefghijklmnopqrstuvwxz"
        Assert.AreEqual(text, converter.Convert(text, null, "24", null))
