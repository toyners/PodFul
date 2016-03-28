namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.Assembly
open NUnit.Framework
open PodFul.Library
open System.IO
open System.Reflection

type ChannelFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\";

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        if Directory.Exists(workingDirectory) then
            Directory.Delete(workingDirectory, true)

        Directory.CreateDirectory(workingDirectory) |> ignore

    [<Test>]
    member public this.``Creating Channel record from RSS file.``() = 
        // Arrange
        let inputPath = workingDirectory + "podcast.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath)
        
        // Act
        let record = ChannelFunctions.readChannelRecordFromRSSFile("RssURL", "Directory", inputPath);

        // Assert
        record |> should not' (be null)
        record.Title |> should equal "Channel Title"
        record.Description |> should equal "Channel Description"
        record.Website |> should equal "Channel Website"
        record.Directory |> should equal "Directory"
        record.Feed |> should equal "RssURL"
        record.Podcasts |> should not' (be null)
        record.Podcasts.Length |> should equal 3

        record.Podcasts.[0].Title |> should equal "Podcast #1 Title"
        record.Podcasts.[0].Description |> should equal "Podcast #1 Description"
        record.Podcasts.[0].URL |> should equal "podcast1.mp3"
        record.Podcasts.[0].FileSize |> should equal 1L
        record.Podcasts.[0].PubDate |> should equal (new System.DateTime(2016, 3, 14, 18, 16, 10))

        record.Podcasts.[1].Title |> should equal "Podcast #2 Title"
        record.Podcasts.[1].Description |> should equal "Podcast #2 Description"
        record.Podcasts.[1].URL |> should equal "podcast2.mp3"
        record.Podcasts.[1].FileSize |> should equal 2L
        record.Podcasts.[1].PubDate |> should equal (new System.DateTime(2016, 3, 7, 17, 40, 54))

        record.Podcasts.[2].Title |> should equal "Podcast #3 Title"
        record.Podcasts.[2].Description |> should equal "Podcast #3 Description"
        record.Podcasts.[2].URL |> should equal "podcast3.mp3"
        record.Podcasts.[2].FileSize |> should equal 3L
        record.Podcasts.[2].PubDate |> should equal (new System.DateTime(2016, 3, 1, 20, 52, 44))
