namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO

type FeedFileStorage_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\FeedFileStorage_IntergrationTests\";

    let feedTitle = "Feed Title"
    let feedDescription = "Feed Description"
    let feedWebsite = "Feed Website"
    let feedDirectory = "Feed Directory"
    let feedFeed = "Feed Feed"
    let feedImageFileName = "Feed Image"

    let podcastTitle = "Podcast #1 Title"
    let podcastDescription = "Podcast #1 Description"
    let podcastURL = "Podcast1.mp3"
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
            ImageFileName = feedImageFileName
            CreationDateTime = new DateTime(2016, 2, 3)
            Podcasts = 
            [|
                {
                    Title = podcastTitle
                    Description = podcastDescription
                    URL = podcastURL
                    FileSize = podcastFileSize
                    PubDate = podcastPubDate
                    DownloadDate = new DateTime(2017, 1, 2)
                    ImageFileName = podcastImageFileName
                };           
            |]
        }

    member private this.UpdateFeed (feed : Feed) : Feed =
        {
            Title = feed.Title
            Description = feed.Description
            Website = feed.Website
            Directory = feed.Directory
            URL = feed.URL
            ImageFileName = feed.ImageFileName
            Podcasts = [||]
            CreationDateTime = feed.CreationDateTime
        }

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Feed File Storage is not open by default``() = 
        let feedStorage = FeedFileStorage(String.Empty).Storage()
        feedStorage.IsOpen |> should equal false
        feedStorage.Feeds |> should equal null;

    [<Test>]
    member public this.``Adding a feed creates a file in the directory``() = 

        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        Directory.GetFiles(workingDirectory, "*").Length |> should equal 1
        Directory.GetFiles(workingDirectory, "0_" + feed.Title + ".feed").Length |> should equal 1

    [<Test>]
    member public this.``Adding a feed adds it to the feed storage``() = 

        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        feedStorage.Feeds.Length |> should equal 1
        feedStorage.Feeds.[0] |> should equal feed 

    [<Test>]
    member public this.``Adding the same feed throws meaningful exception``() = 

        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        (fun() -> feedStorage.Add(feed) |> ignore)
        |> should (throwWithMessage "Feed already in storage.") typeof<System.Exception>

    [<Test>]
    member public this.``Removing a feed removes it from the feed storage``() =
    
        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Remove(feed)

        feedStorage.Feeds.Length |> should equal 0

    [<Test>]
    member public this.``Removing a feed removes file from the directory``() =
    
        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Remove(feed)

        Directory.GetFiles(workingDirectory, "*").Length |> should equal 0
        Directory.GetFiles(workingDirectory, "0_" + feed.Title + ".feed").Length |> should equal 0

    [<Test>]
    member public this.``Removing the same feed throws meaningful exception``() =
    
        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Remove(feed)

        (fun() -> feedStorage.Remove(feed) |> ignore)
        |> should (throwWithMessage "Feed cannot be removed because it cannot be found in storage.") typeof<System.Exception>

    [<Test>]
    member public this.``Updating the feed updates the file in the directory``() =

        let originalFeed = this.CreateFeed
        let updatedFeed = this.UpdateFeed originalFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()
        feedStorage.Open()
        feedStorage.Add(originalFeed)

        let filePath = Directory.GetFiles(workingDirectory, "0_" + originalFeed.Title + ".feed").[0]
        let fileInfo = new FileInfo(filePath)
        let addedDateTime = fileInfo.LastWriteTime

        feedStorage.Update(updatedFeed)
        let fileInfo = new FileInfo(filePath)
        let updatedDateTime = fileInfo.LastWriteTime

        updatedDateTime |> should be (greaterThan addedDateTime)

    [<Test>]
    member public this.``Updating the feed creates old feed file in the directory``() =

        let originalFeed = this.CreateFeed
        let updatedFeed = this.UpdateFeed originalFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()
        feedStorage.Open()
        feedStorage.Add(originalFeed)
        feedStorage.Update(updatedFeed)

        Directory.GetFiles(workingDirectory, "*").Length |> should equal 2
        Directory.GetFiles(workingDirectory, "0_" + originalFeed.Title + ".feed.old").Length |> should equal 1

    [<Test>]
    member public this.``Updating the feed updates the feed in storage``() =

        let originalFeed = this.CreateFeed
        let updatedFeed = this.UpdateFeed originalFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()
        feedStorage.Open()
        feedStorage.Add(originalFeed)
        feedStorage.Update(updatedFeed)

        feedStorage.Feeds.Length |> should equal 1
        feedStorage.Feeds.[0].Podcasts.Length |> should equal 0

    [<Test>]
    member public this.``Closing the feed storage removes all feeds from memory``() =

        let feed = this.CreateFeed
        let originalFeedStorage = FeedFileStorage(workingDirectory).Storage()
        originalFeedStorage.Open()
        originalFeedStorage.Add(feed)
        originalFeedStorage.Close()

        originalFeedStorage.Feeds |> should equal null

    [<Test>]
    member public this.``Writing/Reading cycle of Feed``() =

        let feed = this.CreateFeed
        let originalFeedStorage = FeedFileStorage(workingDirectory).Storage()
        originalFeedStorage.Open()
        originalFeedStorage.Add(feed)
        originalFeedStorage.Close()

        let nextFeedStorage = FeedFileStorage(workingDirectory).Storage()
        nextFeedStorage.Open()

        let actualFeed = nextFeedStorage.Feeds.[0]
        actualFeed |> should equal feed
        actualFeed.Title |> should equal feed.Title
        actualFeed.Description |> should equal feed.Description
        actualFeed.Website |> should equal feed.Website
        actualFeed.Directory |> should equal feed.Directory
        actualFeed.URL |> should equal feed.URL
        actualFeed.CreationDateTime |> should equal feed.CreationDateTime

        let actualPodcast = actualFeed.Podcasts.[0]
        actualFeed.Podcasts.Length |> should equal feed.Podcasts.Length
        actualPodcast |> should equal feed.Podcasts.[0]
        actualPodcast.Title |> should equal feed.Podcasts.[0].Title
        actualPodcast.Description |> should equal feed.Podcasts.[0].Description
        actualPodcast.URL |> should equal feed.Podcasts.[0].URL
        actualPodcast.FileSize |> should equal feed.Podcasts.[0].FileSize
        actualPodcast.PubDate |> should equal feed.Podcasts.[0].PubDate
        actualPodcast.DownloadDate |> should equal feed.Podcasts.[0].DownloadDate
