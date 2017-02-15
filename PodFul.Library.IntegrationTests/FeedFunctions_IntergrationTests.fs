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
    let rssFileName = "RSSFile.rss"
    let initialRSSFileName = "Initial RSSFile.rss"
    let finalRSSFileName = "Final RSSFile.rss"
    let initialRSSWithMaximumPodcastsFileName = "Initial RSSFile with Maximum Podcasts.rss"
    let finalRSSWithMaximumPodcastsFileName = "Final RSSFile with Maximum Podcasts.rss"
    let rssWithNoPodcasts = "No Podcasts.rss"
    let rssWithOnePodcast = "One Podcast.rss"
    let rssWithValidImages = "RSS with valid Images.rss"
    let rssContainingOneItemWithMultiplePodcastFiles = "One Item with Muliple Podcast files.rss"

    let feedTitle = "Feed Title"
    let feedDescription = "Feed Description"
    let feedWebsite = "Feed Website"
    let feedImageFileName = "Feed Image"

    let firstPodcastTitle = "Podcast #1 Title"
    let firstPodcastDescription = "Podcast #1 Description"
    let firstPodcastURL = "Podcast1.mp3"
    let firstPodcastFileSize = 1L
    let firstPodcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)
    let firstPodcastImageURL = "Podcast #1 Image"

    let secondPodcastTitle = "Podcast #2 Title"
    let secondPodcastDescription = "Podcast #2 Description"
    let secondPodcastURL = "Podcast2.mp3"
    let secondPodcastFileSize = 2L
    let secondPodcastPubDate = new DateTime(2015, 3, 4, 10, 11, 12)

    let thirdPodcastTitle = "Podcast #3 Title"
    let thirdPodcastURL = "Podcast3.mp3"
    let thirdPodcastFileSize = 3L

    let podcastTitle = "Podcast Title"
    let podcastDescription = "Podcast Description"
    let podcastImageURL = "Podcast Image"
    let podcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Create Feed from RSS url``() =
        let inputPath = workingDirectory + rssFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)
        let feed = TestingSupport.createTestFeed inputPath "DirectoryPath"

        Assert.AreNotEqual(null, feed)
        Assert.AreEqual(feedTitle, feed.Title)
        Assert.AreEqual(feedDescription, feed.Description)
        Assert.AreEqual(feedWebsite, feed.Website)
        Assert.AreEqual("DirectoryPath", feed.Directory)
        Assert.AreEqual(inputPath, feed.URL)
        Assert.AreEqual(feedImageFileName, feed.ImageURL)
        Assert.AreEqual("", feed.ImageFileName)

        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(3, feed.Podcasts.Length)

        Assert.AreEqual(firstPodcastTitle, feed.Podcasts.[0].Title)
        Assert.AreEqual(firstPodcastDescription, feed.Podcasts.[0].Description)
        Assert.AreEqual(firstPodcastURL, feed.Podcasts.[0].URL)
        Assert.AreEqual(firstPodcastImageURL, feed.Podcasts.[0].ImageURL)
        Assert.AreEqual(firstPodcastFileSize, feed.Podcasts.[0].FileDetails.FileSize)
        Assert.AreEqual(firstPodcastPubDate, feed.Podcasts.[0].PubDate)
        Assert.AreEqual(String.Empty, feed.Podcasts.[0].FileDetails.ImageFileName)

        Assert.AreEqual(secondPodcastTitle, feed.Podcasts.[1].Title)
        Assert.AreEqual(secondPodcastDescription, feed.Podcasts.[1].Description)
        Assert.AreEqual(secondPodcastURL, feed.Podcasts.[1].URL)
        Assert.AreEqual(String.Empty, feed.Podcasts.[1].ImageURL)
        Assert.AreEqual(secondPodcastFileSize, feed.Podcasts.[1].FileDetails.FileSize)
        Assert.AreEqual(secondPodcastPubDate, feed.Podcasts.[1].PubDate)
        Assert.AreEqual(String.Empty, feed.Podcasts.[1].FileDetails.ImageFileName)

        Assert.AreEqual(thirdPodcastTitle, feed.Podcasts.[2].Title)
        Assert.AreEqual(String.Empty, feed.Podcasts.[2].Description)
        Assert.AreEqual(thirdPodcastURL, feed.Podcasts.[2].URL)
        Assert.AreEqual(String.Empty, feed.Podcasts.[2].ImageURL)
        Assert.AreEqual(-1, feed.Podcasts.[2].FileDetails.FileSize)
        Assert.AreEqual(FeedFunctions.NoDateTime, feed.Podcasts.[2].PubDate)
        Assert.AreEqual(String.Empty, feed.Podcasts.[2].FileDetails.ImageFileName)

    [<Test>]
    member public this.``Create RSS Feed that contains media content tags``() =
        let inputPath = workingDirectory + "RSSFileWithMediaContent.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("RSSFileWithMediaContent.rss", inputPath)
        let feed = TestingSupport.createTestFeed inputPath "DirectoryPath"

        Assert.AreNotEqual(null, feed)
        Assert.AreEqual(feedTitle, feed.Title)
        Assert.AreEqual(feedDescription, feed.Description)
        Assert.AreEqual(feedWebsite, feed.Website)
        Assert.AreEqual("DirectoryPath", feed.Directory)
        Assert.AreEqual(inputPath, feed.URL)

        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(3, feed.Podcasts.Length)

        Assert.AreEqual(firstPodcastTitle, feed.Podcasts.[0].Title)
        Assert.AreEqual(firstPodcastURL, feed.Podcasts.[0].URL)
        Assert.AreEqual(firstPodcastFileSize, feed.Podcasts.[0].FileDetails.FileSize)

        Assert.AreEqual(secondPodcastTitle, feed.Podcasts.[1].Title)
        Assert.AreEqual(secondPodcastURL, feed.Podcasts.[1].URL)
        Assert.AreEqual(secondPodcastFileSize, feed.Podcasts.[1].FileDetails.FileSize)

        Assert.AreEqual(thirdPodcastTitle, feed.Podcasts.[2].Title)
        Assert.AreEqual(thirdPodcastURL, feed.Podcasts.[2].URL)
        Assert.AreEqual(thirdPodcastFileSize, feed.Podcasts.[2].FileDetails.FileSize)

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

        Assert.AreEqual(String.Empty, feed.ImageFileName)

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

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(3, finalFeed.Podcasts.Length)

        Assert.AreEqual(firstPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[1].FileDetails.DownloadDate)
        Assert.AreEqual(secondPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[2].FileDetails.DownloadDate)

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

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(3, finalFeed.Podcasts.Length)

        Assert.AreEqual(firstPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[1].FileDetails.DownloadDate)
        Assert.AreEqual(secondPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[2].FileDetails.DownloadDate)

    [<Test>]
    member public this.``Update feed with no podcasts from RSS file with no podcasts``() =
        let initialInputPath = workingDirectory + rssWithNoPodcasts

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(0, finalFeed.Podcasts.Length)

    [<Test>]
    member public this.``Update feed with no podcasts from RSS file with podcasts``() =
        let initialInputPath = workingDirectory + rssWithNoPodcasts

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let initialFeed = TestingSupport.createTestFeed initialInputPath "DirectoryPath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithOnePodcast, initialInputPath)
        let finalFeed = TestingSupport.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(1, finalFeed.Podcasts.Length)

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

    [<Test>]
    member public this.``Create feed with multiple podcast files per Item``() =
        let inputPath = workingDirectory + rssContainingOneItemWithMultiplePodcastFiles
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssContainingOneItemWithMultiplePodcastFiles, inputPath)
        let feed = TestingSupport.createTestFeed inputPath "DirectoryPath"

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(3, feed.Podcasts.Length)

        TestingSupport.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty