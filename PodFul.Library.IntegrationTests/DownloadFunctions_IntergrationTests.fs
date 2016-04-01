namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System.IO

type DownloadFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\DownloadFunctions_IntergrationTests\";

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Test``() =
        let url = ""
        let path = ""
        DownloadFunctions.downloadFile(url, path) |> ignore

        File.Exists(path) |> should equal true
