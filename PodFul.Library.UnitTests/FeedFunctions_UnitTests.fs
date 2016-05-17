namespace PodFul.Library.UnitTests

open FsUnit
//open Jabberwocky.Toolkit.Assembly
//open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection

type FeedFunctions_UnitTests() = 

    [<Test>]
    member public this.``Clean text of line break``() =
        FeedFunctions.CleanText "\r\n" |> should equal " "
    
    [<Test>]
    member public this.``Clean text of line break 2``() =
        FeedFunctions.CleanText "\n" |> should equal " "

    [<Test>]
    member public this.``Clean text of multiple spaces``() =
        FeedFunctions.CleanText "  " |> should equal " "

    [<Test>]
    member public this.``Clean text of multiple spaces 2``() =
        FeedFunctions.CleanText "   " |> should equal " "

    [<Test>]
    member public this.``Clean text of <p></p> tags``() =
        FeedFunctions.CleanText "<p> </p>" |> should equal " "
