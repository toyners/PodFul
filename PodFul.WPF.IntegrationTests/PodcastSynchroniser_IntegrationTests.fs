namespace PodFul.WPF.IntegrationTests

open System
open System.IO
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open PodFul.WPF.Miscellaneous

type PodcastSynchroniser_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\PodcastSynchroniser_IntegrationTests\";

    let createPodcastRecord title url =
        {
            Title = title
            Description = "Description"
            URL = url
            ImageURL = "imageURL"
            PubDate = DateTime.MinValue
            FileDetails = 
            {
                FileSize = 0L
                DownloadDate = DateTime.MinValue
                ImageFileName = String.Empty
            }
        }        

    member private this.CreateFeed podcastList =
        {
            Title = "title"
            Description = "description"
            Website = "website"
            Directory = "directory"
            URL = "url"
            ImageURL = "image URL"
            ImageFileName = ""
            Podcasts = podcastList
            CreationDateTime = new DateTime(2016, 2, 3)
            UpdatedDateTime = FeedFunctions.NoDateTime
        }


    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Test``() =

        let filePath = Path.Combine(workingDirectory, "file.mp3")
        let file = File.Create(filePath)
        file.Close()

        let podcast = createPodcastRecord "title" filePath
        let feed = this.CreateFeed [| podcast |]

        let count = PodcastSynchroniser.Synchronise(feed)

        Assert.AreEqual(1, count)