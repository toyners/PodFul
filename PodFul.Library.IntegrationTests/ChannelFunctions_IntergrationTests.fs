﻿namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection

type ChannelFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\ChannelFunctions_IntergrationTests\";

    let channelTitle = "Channel Title"
    let channelDescription = "Channel Description"
    let channelWebsite = "Channel Website"
    let channelDirectory = "Channel Directory"
    let channelFeed = "Channel Feed"

    let firstPodcastTitle = "Podcast #1 Title"
    let firstPodcastDescription = "Podcast #1 Description"
    let firstPodcastURL = "Podcast1.mp3"
    let firstPodcastFileSize = 1L
    let firstPodcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)

    let secondPodcastTitle = "Podcast #2 Title"
    let secondPodcastDescription = "Podcast #2 Description"
    let secondPodcastURL = "Podcast2.mp3"
    let secondPodcastFileSize = 2L
    let secondPodcastPubDate = new DateTime(2015, 3, 4, 10, 11, 12)

    let thirdPodcastTitle = "Podcast #3 Title"
    let thirdPodcastDescription = "Podcast #3 Description"
    let thirdPodcastURL = "Podcast3.mp3"
    let thirdPodcastFileSize = 3L
    let thirdPodcastPubDate = new DateTime(2016, 5, 6, 15, 16, 17)

    member private this.CreateChannelRecord =
        {
            Title = channelTitle
            Description = channelDescription
            Website = channelWebsite
            Directory = channelDirectory
            Feed = channelFeed
            Podcasts = 
            [|
                {
                    Title = firstPodcastTitle
                    Description = firstPodcastDescription
                    URL = firstPodcastURL
                    FileSize = firstPodcastFileSize
                    PubDate = firstPodcastPubDate
                };
                {
                    Title = secondPodcastTitle
                    Description = secondPodcastDescription
                    URL = secondPodcastURL
                    FileSize = secondPodcastFileSize
                    PubDate = secondPodcastPubDate
                };
                {
                    Title = thirdPodcastTitle
                    Description = thirdPodcastDescription
                    URL = thirdPodcastURL
                    FileSize = thirdPodcastFileSize
                    PubDate = thirdPodcastPubDate
                };            
            |]
        }

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Creating Channel record from RSS file``() = 
        // Arrange
        let inputPath = workingDirectory + "podcast.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath)
        
        // Act
        let record = ChannelFunctions.readChannelRecordFromRSSFile(channelFeed, channelDirectory, inputPath);

        // Assert
        record |> should not' (be null)
        record.Title |> should equal channelTitle
        record.Description |> should equal channelDescription
        record.Website |> should equal channelWebsite
        record.Directory |> should equal channelDirectory
        record.Feed |> should equal channelFeed
        record.Podcasts |> should not' (be null)
        record.Podcasts.Length |> should equal 3

        record.Podcasts.[0].Title |> should equal firstPodcastTitle
        record.Podcasts.[0].Description |> should equal firstPodcastDescription
        record.Podcasts.[0].URL |> should equal firstPodcastURL
        record.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        record.Podcasts.[0].PubDate |> should equal firstPodcastPubDate

        record.Podcasts.[1].Title |> should equal secondPodcastTitle
        record.Podcasts.[1].Description |> should equal secondPodcastDescription
        record.Podcasts.[1].URL |> should equal secondPodcastURL
        record.Podcasts.[1].FileSize |> should equal secondPodcastFileSize
        record.Podcasts.[1].PubDate |> should equal secondPodcastPubDate

        record.Podcasts.[2].Title |> should equal thirdPodcastTitle
        record.Podcasts.[2].Description |> should equal thirdPodcastDescription
        record.Podcasts.[2].URL |> should equal thirdPodcastURL
        record.Podcasts.[2].FileSize |> should equal thirdPodcastFileSize
        record.Podcasts.[2].PubDate |> should equal thirdPodcastPubDate

    [<Test>]
    member public this.``Writing/Reading cycle of Channel record``() = 
        let testRecord = this.CreateChannelRecord
        let outputPath = workingDirectory + "record.txt";
        ChannelFunctions.writeChannelRecordToFile(testRecord, outputPath)

        File.Exists(outputPath) |> should be True

        let inputPath = outputPath
        let resultRecord = ChannelFunctions.readChannelRecordFromFile(inputPath)

        resultRecord |> should not' (be sameAs testRecord)

        resultRecord.Title |> should equal channelTitle
        resultRecord.Description |> should equal channelDescription
        resultRecord.Website |> should equal channelWebsite
        resultRecord.Directory |> should equal channelDirectory
        resultRecord.Feed |> should equal channelFeed

        resultRecord.Podcasts |> should not' (be null)
        resultRecord.Podcasts.Length |> should equal 3

        resultRecord.Podcasts.[0].Title |> should equal firstPodcastTitle
        resultRecord.Podcasts.[0].Description |> should equal firstPodcastDescription
        resultRecord.Podcasts.[0].URL |> should equal firstPodcastURL
        resultRecord.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        resultRecord.Podcasts.[0].PubDate |> should equal firstPodcastPubDate

        resultRecord.Podcasts.[1].Title |> should equal secondPodcastTitle
        resultRecord.Podcasts.[1].Description |> should equal secondPodcastDescription
        resultRecord.Podcasts.[1].URL |> should equal secondPodcastURL
        resultRecord.Podcasts.[1].FileSize |> should equal secondPodcastFileSize
        resultRecord.Podcasts.[1].PubDate |> should equal secondPodcastPubDate

        resultRecord.Podcasts.[2].Title |> should equal thirdPodcastTitle
        resultRecord.Podcasts.[2].Description |> should equal thirdPodcastDescription
        resultRecord.Podcasts.[2].URL |> should equal thirdPodcastURL
        resultRecord.Podcasts.[2].FileSize |> should equal thirdPodcastFileSize
        resultRecord.Podcasts.[2].PubDate |> should equal thirdPodcastPubDate