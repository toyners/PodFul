namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection

type FeedFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\FeedFunctions_IntergrationTests\";

    let rssFileName = "RSSFile.rss"
    let initialRSSFileName = "Initial RSSFile.rss"
    let finalRSSFileName = "Final RSSFile.rss"
    let initialRSSWithMaximumPodcastsFileName = "Initial RSSFile with Maximum Podcasts.rss"
    let finalRSSWithMaximumPodcastsFileName = "Final RSSFile with Maximum Podcasts.rss"

    let feedTitle = "Feed Title"
    let feedDescription = "Feed Description"
    let feedWebsite = "Feed Website"
    let feedImageFileName = "Feed Image"

    let firstPodcastTitle = "Podcast #1 Title"
    let firstPodcastDescription = "Podcast #1 Description"
    let firstPodcastURL = "Podcast1.mp3"
    let firstPodcastFileSize = 1L
    let firstPodcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)
    let firstPodcastImageFileName = "Podcast #1 Image"

    let secondPodcastTitle = "Podcast #2 Title"
    let secondPodcastDescription = "Podcast #2 Description"
    let secondPodcastURL = "Podcast2.mp3"
    let secondPodcastFileSize = 2L
    let secondPodcastPubDate = new DateTime(2015, 3, 4, 10, 11, 12)

    let thirdPodcastTitle = "Podcast #3 Title"
    let thirdPodcastURL = "Podcast3.mp3"
    let thirdPodcastFileSize = 3L

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Create Feed from RSS url``() =
        let inputPath = workingDirectory + rssFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)
        let feed = FeedFunctions.CreateFeed inputPath "DirectoryPath"

        feed |> should not' (equal null)
        feed.Title |> should equal feedTitle
        feed.Description |> should equal feedDescription
        feed.Website |> should equal feedWebsite
        feed.Directory |> should equal "DirectoryPath"
        feed.URL |> should equal inputPath
        feed.ImageFileName |> should equal feedImageFileName

        feed.Podcasts |> should not' (be null)
        feed.Podcasts.Length |> should equal 3

        feed.Podcasts.[0].Title |> should equal firstPodcastTitle
        feed.Podcasts.[0].Description |> should equal firstPodcastDescription
        feed.Podcasts.[0].URL |> should equal firstPodcastURL
        feed.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        feed.Podcasts.[0].PubDate |> should equal firstPodcastPubDate
        feed.Podcasts.[0].ImageFileName |> should equal firstPodcastImageFileName

        feed.Podcasts.[1].Title |> should equal secondPodcastTitle
        feed.Podcasts.[1].Description |> should equal secondPodcastDescription
        feed.Podcasts.[1].URL |> should equal secondPodcastURL
        feed.Podcasts.[1].FileSize |> should equal secondPodcastFileSize
        feed.Podcasts.[1].PubDate |> should equal secondPodcastPubDate
        feed.Podcasts.[1].ImageFileName |> should equal String.Empty

        feed.Podcasts.[2].Title |> should equal thirdPodcastTitle
        feed.Podcasts.[2].Description |> should equal ""
        feed.Podcasts.[2].URL |> should equal thirdPodcastURL
        feed.Podcasts.[2].FileSize |> should equal -1
        feed.Podcasts.[2].PubDate |> should equal DateTime.MinValue
        feed.Podcasts.[2].ImageFileName |> should equal String.Empty

    [<Test>]
    member public this.``Create RSS Feed that contains media content tags``() =
        let inputPath = workingDirectory + "RSSFileWithMediaContent.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("RSSFileWithMediaContent.rss", inputPath)
        let feed = FeedFunctions.CreateFeed inputPath "DirectoryPath"

        feed |> should not' (equal null)
        feed.Title |> should equal feedTitle
        feed.Description |> should equal feedDescription
        feed.Website |> should equal feedWebsite
        feed.Directory |> should equal "DirectoryPath"
        feed.URL |> should equal inputPath

        feed.Podcasts |> should not' (be null)
        feed.Podcasts.Length |> should equal 3

        feed.Podcasts.[0].Title |> should equal firstPodcastTitle
        feed.Podcasts.[0].URL |> should equal firstPodcastURL
        feed.Podcasts.[0].FileSize |> should equal firstPodcastFileSize

        feed.Podcasts.[1].Title |> should equal secondPodcastTitle
        feed.Podcasts.[1].URL |> should equal secondPodcastURL
        feed.Podcasts.[1].FileSize |> should equal secondPodcastFileSize

        feed.Podcasts.[2].Title |> should equal thirdPodcastTitle
        feed.Podcasts.[2].URL |> should equal thirdPodcastURL
        feed.Podcasts.[2].FileSize |> should equal thirdPodcastFileSize

    [<Test>]
    [<TestCase("RSSFile with Missing Image.rss")>]
    [<TestCase("RSSFile with Empty Image 1.rss")>]
    [<TestCase("RSSFile with Empty Image 2.rss")>]
    [<TestCase("RSSFile with Empty URL 1.rss")>]
    [<TestCase("RSSFile with Empty URL 2.rss")>]
    member public this.``Create feed from RSS file with no feed image data``(feedFileName : string) =
        let inputPath = workingDirectory + feedFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, inputPath)
        let feed = FeedFunctions.CreateFeed inputPath "DirectoryPath"

        feed.ImageFileName |> should equal String.Empty

    [<Test>]
    member public this.``Update feed from RSS file``() =
        let initialInputPath = workingDirectory + initialRSSFileName
        let firstDateTime = new DateTime(2015, 1, 2, 8, 9, 10)
        let secondDateTime = new DateTime(2016, 3, 4, 11, 12, 13)

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSFileName, initialInputPath)
        let initialFeed = FeedFunctions.CreateFeed initialInputPath "DirectoryPath"
        initialFeed.Podcasts.[0] <- Podcast.SetDownloadDate firstDateTime initialFeed.Podcasts.[0]
        initialFeed.Podcasts.[1] <- Podcast.SetDownloadDate secondDateTime initialFeed.Podcasts.[1]

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSFileName, initialInputPath)
        let finalFeed = FeedFunctions.UpdateFeed initialFeed

        finalFeed |> should equal initialFeed
        finalFeed.Podcasts.Length |> should equal 3

        finalFeed.Podcasts.[1].DownloadDate |> should equal initialFeed.Podcasts.[0].DownloadDate
        finalFeed.Podcasts.[2].DownloadDate |> should equal initialFeed.Podcasts.[1].DownloadDate

    [<Test>]
    member public this.``Update feed from RSS file with maximum number of podcasts``() =
        let initialInputPath = workingDirectory + initialRSSWithMaximumPodcastsFileName
        let firstDateTime = new DateTime(2015, 1, 2, 8, 9, 10)
        let secondDateTime = new DateTime(2016, 3, 4, 11, 12, 13)

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSWithMaximumPodcastsFileName, initialInputPath)
        let initialFeed = FeedFunctions.CreateFeed initialInputPath "DirectoryPath"
        initialFeed.Podcasts.[0] <- Podcast.SetDownloadDate firstDateTime initialFeed.Podcasts.[0]
        initialFeed.Podcasts.[1] <- Podcast.SetDownloadDate secondDateTime initialFeed.Podcasts.[1]

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSWithMaximumPodcastsFileName, initialInputPath)
        let finalFeed = FeedFunctions.UpdateFeed initialFeed

        finalFeed |> should equal initialFeed
        finalFeed.Podcasts.Length |> should equal 3

        finalFeed.Podcasts.[1].DownloadDate |> should equal initialFeed.Podcasts.[0].DownloadDate
        finalFeed.Podcasts.[2].DownloadDate |> should equal initialFeed.Podcasts.[1].DownloadDate

    [<Test>]
    member public this.``Remove multi-byte characters``() = 
        let inputPath = workingDirectory + "Multibyte file.txt"
        let (bytes : Byte[]) = [| 226uy (*; 8364uy; 8482*) |]
        File.WriteAllBytes(inputPath, bytes)