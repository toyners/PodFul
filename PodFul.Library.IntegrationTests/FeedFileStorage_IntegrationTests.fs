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

    let downloadDate = new DateTime(2017, 1, 2)

    member private this.CreateFeed =
        {
            Title = feedTitle
            Description = feedDescription
            Website = feedWebsite
            Directory = feedDirectory
            URL = feedFeed
            Podcasts = 
            [|
                {
                    Title = firstPodcastTitle
                    Description = firstPodcastDescription
                    URL = firstPodcastURL
                    FileSize = firstPodcastFileSize
                    PubDate = firstPodcastPubDate
                    DownloadDate = downloadDate
                };
                {
                    Title = secondPodcastTitle
                    Description = secondPodcastDescription
                    URL = secondPodcastURL
                    FileSize = secondPodcastFileSize
                    PubDate = secondPodcastPubDate
                    DownloadDate = downloadDate
                };
                {
                    Title = thirdPodcastTitle
                    Description = thirdPodcastDescription
                    URL = thirdPodcastURL
                    FileSize = -1L
                    PubDate = DateTime.MinValue
                    DownloadDate = downloadDate
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
            Podcasts = [||]
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
