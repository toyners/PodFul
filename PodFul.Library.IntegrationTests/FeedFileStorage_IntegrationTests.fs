namespace PodFul.Library.IntegrationTests

open FsUnit
//open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO
//open System.Reflection

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
                    Description = ""
                    URL = thirdPodcastURL
                    FileSize = -1L
                    PubDate = DateTime.MinValue
                    DownloadDate = downloadDate
                };            
            |]
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
        Directory.GetFiles(workingDirectory, "*" + feed.Title + "*").Length |> should equal 1

    [<Test>]
    member public this.``Adding a feed adds it to the feed storage``() = 

        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)

        feedStorage.Feeds.Length |> should equal 1
        feedStorage.Feeds.[0] |> should equal feed 

    [<Test>]
    member public this.``Adding the same feed does not add it to the feed storage``() = 

        let feed = this.CreateFeed
        let feedStorage = FeedFileStorage(workingDirectory).Storage()

        feedStorage.Open()
        feedStorage.Add(feed)
        feedStorage.Add(feed)

        feedStorage.Feeds.Length |> should equal 1
        feedStorage.Feeds.[0] |> should equal feed 

    [<Test>]
    member public this.``Removing a feed removes  ``() = ignore