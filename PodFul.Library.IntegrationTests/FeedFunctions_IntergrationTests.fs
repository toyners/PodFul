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

    let createTestFeed url directoryPath =
        FeedFunctions.CreateFeed url directoryPath null CancellationToken.None

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null CancellationToken.None

    [<Test>]
    member public this.``Update feed with local image paths set``() =
        let initialInputPath = workingDirectory + feedFileName
        let feedImageName = "LocalFeedImagePath"
        let podcastImageName = "LocalPodcastImagePath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, initialInputPath)
        let mutable initialFeed = createTestFeed initialInputPath "DirectoryPath"
        initialFeed <- Feed.SetImageFileName initialFeed feedImageName
        initialFeed.Podcasts.[0].SetImageFileName podcastImageName
        
        let finalFeed = updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(feedImageName, finalFeed.ImageFileName)
        Assert.AreEqual(podcastImageName, finalFeed.Podcasts.[0].FileDetails.ImageFileName)
