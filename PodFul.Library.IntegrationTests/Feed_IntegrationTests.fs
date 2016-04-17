namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System.Reflection

type Feed_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Feed_IntergrationTests\";

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Two different feed records are equal because of same feed.``() =
        let inputPath = workingDirectory + "podcast.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath)

        let feed1 = FeedFunctions.CreateFeed inputPath "DirectoryPath1"
        let feed2 = FeedFunctions.CreateFeed inputPath "DirectoryPath2"

        feed1 = feed2 |> should equal true

    [<Test>]
    member public this.``Two different feed records are not equal because of different feeds.``() =
        let inputPath1 = workingDirectory + "podcast1.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath1)

        let inputPath2 = workingDirectory + "podcast2.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath2)

        let feed1 = FeedFunctions.CreateFeed inputPath1 "DirectoryPath1"
        let feed2 = FeedFunctions.CreateFeed inputPath2 "DirectoryPath2"

        feed1 = feed2 |> should equal false