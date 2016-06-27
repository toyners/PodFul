namespace PodFul.WPF.UnitTests

open System
open FsUnit
open NUnit.Framework
open PodFul.WPF

type TextTruncator_UnitTests() =

    [<Test>]
    member public this.``Value is not of string type so empty string is returned.``() =
        let converter = new TextTruncator(17)
        converter.Convert(new Object(), null, null, null) |> should equal String.Empty;

    [<Test>]
    member public this.``Text length is below maximum character limit so full text is returned.``() =
        let converter = new TextTruncator(17)
        converter.Convert("The red fox", null, null, null) |> should equal "The red fox";

    [<Test>]
    member public this.``Text length is at maximum character limit so full text is returned.``() =
        let converter = new TextTruncator(17)
        converter.Convert("The quick red fox", null, null, null) |> should equal "The quick red fox";

    [<Test>]
    member public this.``Text length is above maximum character limit so truncated text is returned.``() =
        let converter = new TextTruncator(17)
        converter.Convert("The quick red fox jumped over the lazy brown dog", null, null, null) |> should equal "The quick red ...";

    [<Test>]
    member public this.``Word is split by maximum character limit so is not included in truncated text.``() =
        let converter = new TextTruncator(16)
        converter.Convert("The quick red fox jumped over the lazy brown dog", null, null, null) |> should equal "The quick ...";

