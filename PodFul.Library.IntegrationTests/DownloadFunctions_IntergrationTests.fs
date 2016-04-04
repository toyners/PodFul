namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.IO
open Jabberwocky.Toolkit.Assembly
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection

type DownloadFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\DownloadFunctions_IntergrationTests\"

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

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Read RSS url into channel record``() =
        let inputPath = workingDirectory + "podcast.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath)
        let feed = DownloadFunctions.DownloadRSSFeed inputPath

        feed |> should not' (equal null)
        feed.Title |> should equal channelTitle
        feed.Description |> should equal channelDescription
        feed.Website |> should equal channelWebsite
        feed.Directory |> should equal null
        feed.Feed |> should equal null

        feed.Podcasts |> should not' (be null)
        feed.Podcasts.Length |> should equal 3

        feed.Podcasts.[0].Title |> should equal firstPodcastTitle
        feed.Podcasts.[0].Description |> should equal firstPodcastDescription
        feed.Podcasts.[0].URL |> should equal firstPodcastURL
        feed.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        feed.Podcasts.[0].PubDate |> should equal firstPodcastPubDate

        feed.Podcasts.[1].Title |> should equal secondPodcastTitle
        feed.Podcasts.[1].Description |> should equal secondPodcastDescription
        feed.Podcasts.[1].URL |> should equal secondPodcastURL
        feed.Podcasts.[1].FileSize |> should equal secondPodcastFileSize
        feed.Podcasts.[1].PubDate |> should equal secondPodcastPubDate

        feed.Podcasts.[2].Title |> should equal thirdPodcastTitle
        feed.Podcasts.[2].Description |> should equal thirdPodcastDescription
        feed.Podcasts.[2].URL |> should equal thirdPodcastURL
        feed.Podcasts.[2].FileSize |> should equal thirdPodcastFileSize
        feed.Podcasts.[2].PubDate |> should equal thirdPodcastPubDate
