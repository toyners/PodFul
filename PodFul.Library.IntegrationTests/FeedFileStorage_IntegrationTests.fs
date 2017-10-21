namespace PodFul.Library.IntegrationTests

open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System
open System.IO

type FeedFileStorage_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\FeedFileStorage_IntergrationTests\";

    let feedTitle = "Feed Title"
    let feedDescription = "Feed Description"
    let feedWebsite = "Feed Website"
    let feedDirectory = "Feed Directory"
    let feedFeed = "Feed Feed"
    let feedImageURL = "Feed Image URL"
    let feedImageFileName = "Feed Image"

    let podcastTitle = "Podcast #1 Title"
    let podcastDescription = "Podcast #1 Description"
    let podcastURL = "Podcast1.mp3"
    let podcastImageURL = "Podcast #1 Image URL"
    let podcastFileSize = 1L
    let podcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)
    let podcastImageFileName = "Podcast #1 Image"

    member private this.CreateFeed =
        {
            Title = feedTitle
            Description = feedDescription
            Website = feedWebsite
            Directory = feedDirectory
            URL = feedFeed
            ImageURL = feedImageURL
            ImageFileName = feedImageFileName
            CreationDateTime = new DateTime(2016, 2, 3)
            UpdatedDateTime = new DateTime(2017, 4, 5)
            DoScan = true
            CompleteDownloadsOnScan = true
            DeliverDownloadsOnScan = true
            Podcasts = 
            [|
                Setup.createTestPodcast podcastTitle podcastDescription podcastURL podcastPubDate podcastFileSize (new DateTime(2017, 1, 2)) podcastImageFileName podcastImageURL String.Empty
            |]
        }

    member private this.UpdateFeed (feed : Feed) : Feed =
        {
            Title = feed.Title
            Description = feed.Description
            Website = feed.Website
            Directory = feed.Directory
            URL = feed.URL
            ImageURL = feed.ImageURL
            ImageFileName = feed.ImageFileName
            Podcasts = [||]
            CreationDateTime = feed.CreationDateTime
            UpdatedDateTime = feed.UpdatedDateTime
            DoScan = true
            CompleteDownloadsOnScan = true
            DeliverDownloadsOnScan = true
        }

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Feed File Storage is not open by default``() = 
        let feedStorage = JSONFileStorage(String.Empty).Storage()
        Assert.AreEqual(false, feedStorage.IsOpen)
        Assert.AreEqual(null, feedStorage.Feeds)

    [<Test>]
    member public this.``Adding a feed creates a file in the directory``() = 

        let feed = this.CreateFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        Assert.AreEqual(1, Directory.GetFiles(workingDirectory, "*").Length)
        Assert.AreEqual(1, Directory.GetFiles(workingDirectory, feed.Title + "_*.feed").Length)

    [<Test>]
    member public this.``Adding a feed adds it to the feed storage``() = 

        let feed = this.CreateFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        Assert.AreEqual(1, feedStorage.Feeds.Length)
        Assert.AreEqual(feed, feedStorage.Feeds.[0]) 

    [<Test>]
    member public this.``Adding the same feed throws meaningful exception``() = 

        let feed = this.CreateFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        let mutable testSuccessful = false
        try
            feedStorage.Add(feed)
        with
        | _ as e ->
            Assert.AreEqual("Feed already in storage.", e.Message)
            testSuccessful <- true

        Assert.AreEqual(true, testSuccessful)

    [<Test>]
    member public this.``Removing a feed removes it from the feed storage``() =
    
        let feed = this.CreateFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Remove(feed)

        Assert.AreEqual(0, feedStorage.Feeds.Length)

    [<Test>]
    member public this.``Removing a feed removes file from the directory``() =
    
        let feed = this.CreateFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Remove(feed)

        Assert.AreEqual(0, Directory.GetFiles(workingDirectory, "*").Length)
        Assert.AreEqual(0, Directory.GetFiles(workingDirectory, "0_" + feed.Title + ".feed").Length)

    [<Test>]
    member public this.``Removing the same feed throws meaningful exception``() =
    
        let feed = this.CreateFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Remove(feed)

        let mutable testSuccessful = false
        try
            feedStorage.Remove(feed)
        with
        | _ as e ->
            Assert.AreEqual("Feed cannot be removed because it cannot be found in storage.", e.Message)
            testSuccessful <- true

        Assert.AreEqual(true, testSuccessful)

    [<Test>]
    member public this.``Updating the feed updates the file in the directory``() =

        let originalFeed = this.CreateFeed
        let updatedFeed = this.UpdateFeed originalFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()
        feedStorage.Open()
        feedStorage.Add(originalFeed)

        let filePath = Directory.GetFiles(workingDirectory, originalFeed.Title + "_*.feed").[0]
        let fileInfo = new FileInfo(filePath)
        let addedDateTime = fileInfo.LastWriteTime

        feedStorage.Update(updatedFeed)
        let fileInfo = new FileInfo(filePath)
        let updatedDateTime = fileInfo.LastWriteTime

        let isGreater = updatedDateTime > addedDateTime

        Assert.AreEqual(true, isGreater)

    [<Test>]
    member public this.``Updating the feed creates old feed file in the directory``() =

        let originalFeed = this.CreateFeed
        let updatedFeed = this.UpdateFeed originalFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()
        feedStorage.Open()
        feedStorage.Add(originalFeed)
        feedStorage.Update(updatedFeed)

        Assert.AreEqual(2, Directory.GetFiles(workingDirectory, "*").Length)
        Assert.AreEqual(1, Directory.GetFiles(workingDirectory, originalFeed.Title + "_*.feed.old").Length)

    [<Test>]
    member public this.``Updating the feed updates the feed in storage``() =

        let originalFeed = this.CreateFeed
        let updatedFeed = this.UpdateFeed originalFeed
        let feedStorage = JSONFileStorage(workingDirectory).Storage()
        feedStorage.Open()
        feedStorage.Add(originalFeed)
        feedStorage.Update(updatedFeed)

        Assert.AreEqual(1, feedStorage.Feeds.Length)
        Assert.AreEqual(0, feedStorage.Feeds.[0].Podcasts.Length)

    [<Test>]
    member public this.``Closing the feed storage removes all feeds from memory``() =

        let feed = this.CreateFeed
        let originalFeedStorage = JSONFileStorage(workingDirectory).Storage()
        originalFeedStorage.Open()
        originalFeedStorage.Add(feed)
        originalFeedStorage.Close()

        Assert.AreEqual(null, originalFeedStorage.Feeds)

    [<Test>]
    member public this.``Writing/Reading cycle of Feed``() =

        let feed = this.CreateFeed
        let originalFeedStorage = JSONFileStorage(workingDirectory).Storage()
        originalFeedStorage.Open()
        originalFeedStorage.Add(feed)
        originalFeedStorage.Close()

        let nextFeedStorage = JSONFileStorage(workingDirectory).Storage()
        nextFeedStorage.Open()

        let actualFeed = nextFeedStorage.Feeds.[0]
        Assert.AreEqual(feed, actualFeed)
        Assert.AreEqual(feed.Title, actualFeed.Title)
        Assert.AreEqual(feed.Description, actualFeed.Description)
        Assert.AreEqual(feed.Website, actualFeed.Website)
        Assert.AreEqual(feed.Directory, actualFeed.Directory)
        Assert.AreEqual(feed.URL, actualFeed.URL)
        Assert.AreEqual(feed.CreationDateTime, actualFeed.CreationDateTime)
        Assert.AreEqual(feed.UpdatedDateTime, actualFeed.UpdatedDateTime)

        let actualPodcast = actualFeed.Podcasts.[0]
        Assert.AreEqual(feed.Podcasts.Length, actualFeed.Podcasts.Length)
        Assert.AreEqual(feed.Podcasts.[0], actualPodcast)
        Assert.AreEqual(feed.Podcasts.[0].Title, actualPodcast.Title)
        Assert.AreEqual(feed.Podcasts.[0].Description, actualPodcast.Description)
        Assert.AreEqual(feed.Podcasts.[0].URL, actualPodcast.URL)
        Assert.AreEqual(feed.Podcasts.[0].PubDate, actualPodcast.PubDate)
        Assert.AreEqual(feed.Podcasts.[0].FileDetails.FileSize, actualPodcast.FileDetails.FileSize)
        Assert.AreEqual(feed.Podcasts.[0].FileDetails.DownloadDate, actualPodcast.FileDetails.DownloadDate)




        