namespace PodFul.Library.IntegrationTests

open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection
open System.Threading

type FeedFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\FeedFunctions_IntergrationTests\";

    let feedFileName = "Feed.rss"

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

(*    [<Test>]
    member public this.``Create Feed from RSS url``() =
        let inputPath = workingDirectory + rssFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)
        let feed = TestingSupport.createTestFeed inputPath "DirectoryPath"

        Assert.AreNotEqual(null, feed)
        feed.Title |> should equal feedTitle
        feed.Description |> should equal feedDescription
        feed.Website |> should equal feedWebsite
        feed.Directory |> should equal "DirectoryPath"
        feed.URL |> should equal inputPath
        feed.ImageURL |> should equal feedImageFileName
        feed.ImageFileName |> should equal ""

        feed.Podcasts |> should not' (be null)
        feed.Podcasts.Length |> should equal 3

        feed.Podcasts.[0].Title |> should equal firstPodcastTitle
        feed.Podcasts.[0].Description |> should equal firstPodcastDescription
        feed.Podcasts.[0].URL |> should equal firstPodcastURL
        feed.Podcasts.[0].ImageURL |> should equal firstPodcastImageURL
        feed.Podcasts.[0].FileDetails.FileSize |> should equal firstPodcastFileSize
        feed.Podcasts.[0].PubDate |> should equal firstPodcastPubDate
        feed.Podcasts.[0].FileDetails.ImageFileName |> should equal String.Empty

        feed.Podcasts.[1].Title |> should equal secondPodcastTitle
        feed.Podcasts.[1].Description |> should equal secondPodcastDescription
        feed.Podcasts.[1].URL |> should equal secondPodcastURL
        feed.Podcasts.[1].ImageURL |> should equal String.Empty
        feed.Podcasts.[1].FileDetails.FileSize |> should equal secondPodcastFileSize
        feed.Podcasts.[1].PubDate |> should equal secondPodcastPubDate
        feed.Podcasts.[1].FileDetails.ImageFileName |> should equal String.Empty

        feed.Podcasts.[2].Title |> should equal thirdPodcastTitle
        feed.Podcasts.[2].Description |> should equal String.Empty
        feed.Podcasts.[2].URL |> should equal thirdPodcastURL
        feed.Podcasts.[2].ImageURL |> should equal String.Empty
        feed.Podcasts.[2].FileDetails.FileSize |> should equal -1
        feed.Podcasts.[2].PubDate |> should equal FeedFunctions.NoDateTime
        feed.Podcasts.[2].FileDetails.ImageFileName |> should equal String.Empty

    [<Test>]
    member public this.``Create RSS Feed that contains media content tags``() =
        let inputPath = workingDirectory + "RSSFileWithMediaContent.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("RSSFileWithMediaContent.rss", inputPath)
        let feed = TestingSupport.createTestFeed inputPath "DirectoryPath"

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
        feed.Podcasts.[0].FileDetails.FileSize |> should equal firstPodcastFileSize

        feed.Podcasts.[1].Title |> should equal secondPodcastTitle
        feed.Podcasts.[1].URL |> should equal secondPodcastURL
        feed.Podcasts.[1].FileDetails.FileSize |> should equal secondPodcastFileSize

        feed.Podcasts.[2].Title |> should equal thirdPodcastTitle
        feed.Podcasts.[2].URL |> should equal thirdPodcastURL
        feed.Podcasts.[2].FileDetails.FileSize |> should equal thirdPodcastFileSize

    [<Test>]
    [<TestCase("RSSFile with Missing Image.rss")>]
    [<TestCase("RSSFile with Empty Image 1.rss")>]
    [<TestCase("RSSFile with Empty Image 2.rss")>]
    [<TestCase("RSSFile with Empty URL 1.rss")>]
    [<TestCase("RSSFile with Empty URL 2.rss")>]
    member public this.``Create feed from RSS file with no feed image data``(feedFileName : string) =
        let inputPath = workingDirectory + feedFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, inputPath)
        let feed = TestingSupport.createTestFeed inputPath "DirectoryPath"

        feed.ImageFileName |> should equal String.Empty

    [<Test>]
    member public this.``Update feed from RSS file``() =
        let initialInputPath = workingDirectory + initialRSSFileName
        let firstDateTime = new DateTime(2015, 1, 2, 8, 9, 10)
        let secondDateTime = new DateTime(2016, 3, 4, 11, 12, 13)

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSFileName, initialInputPath)
        let initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"
        
        let firstPodcast = initialFeed.Podcasts.[0]
        firstPodcast.SetFileDetails firstPodcast.FileDetails.FileSize firstDateTime
        
        let secondPodcast = initialFeed.Podcasts.[1]
        secondPodcast.SetFileDetails secondPodcast.FileDetails.FileSize secondDateTime

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSFileName, initialInputPath)
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        finalFeed |> should equal initialFeed
        finalFeed.Podcasts.Length |> should equal 3

        finalFeed.Podcasts.[1].FileDetails.DownloadDate |> should equal firstPodcast.FileDetails.DownloadDate
        finalFeed.Podcasts.[2].FileDetails.DownloadDate |> should equal secondPodcast.FileDetails.DownloadDate

    [<Test>]
    member public this.``Update feed from RSS file with maximum number of podcasts``() =
        let initialInputPath = workingDirectory + initialRSSWithMaximumPodcastsFileName
        let firstDateTime = new DateTime(2015, 1, 2, 8, 9, 10)
        let secondDateTime = new DateTime(2016, 3, 4, 11, 12, 13)

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSWithMaximumPodcastsFileName, initialInputPath)
        let initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"
        
        let firstPodcast = initialFeed.Podcasts.[0]
        firstPodcast.SetFileDetails firstPodcast.FileDetails.FileSize firstDateTime
        
        let secondPodcast = initialFeed.Podcasts.[1]
        secondPodcast.SetFileDetails secondPodcast.FileDetails.FileSize secondDateTime

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSWithMaximumPodcastsFileName, initialInputPath)
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        finalFeed |> should equal initialFeed
        finalFeed.Podcasts.Length |> should equal 3

        finalFeed.Podcasts.[1].FileDetails.DownloadDate |> should equal firstPodcast.FileDetails.DownloadDate
        finalFeed.Podcasts.[2].FileDetails.DownloadDate |> should equal secondPodcast.FileDetails.DownloadDate

    [<Test>]
    member public this.``Update feed with no podcasts from RSS file with no podcasts``() =
        let initialInputPath = workingDirectory + rssWithNoPodcasts

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        finalFeed |> should equal initialFeed
        finalFeed.Podcasts.Length |> should equal 0

    [<Test>]
    member public this.``Update feed with no podcasts from RSS file with podcasts``() =
        let initialInputPath = workingDirectory + rssWithNoPodcasts

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithOnePodcast, initialInputPath)
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        finalFeed |> should equal initialFeed
        finalFeed.Podcasts.Length |> should equal 1*)

    [<Test>]
    member public this.``Update feed with local image paths set``() =
        let initialInputPath = workingDirectory + feedFileName
        let feedImageName = "LocalFeedImagePath"
        let podcastImageName = "LocalPodcastImagePath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, initialInputPath)
        let mutable initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"
        initialFeed <- Feed.SetImageFileName initialFeed feedImageName
        initialFeed.Podcasts.[0].SetImageFileName podcastImageName
        
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(feedImageName, finalFeed.ImageFileName)
        Assert.AreEqual(podcastImageName, finalFeed.Podcasts.[0].FileDetails.ImageFileName)
