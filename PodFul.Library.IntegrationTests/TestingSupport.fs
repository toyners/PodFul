namespace PodFul.Library.IntegrationTests

open PodFul.Library
open System.Threading
open NUnit.Framework

module public TestingSupport = 

    let createTestFeed url directoryPath =
        FeedFunctions.CreateFeed url null directoryPath null CancellationToken.None

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null null CancellationToken.None

    let assertPodcastIsCorrect (podcast : Podcast) title description url imageURL fileSize pubDate imageFileName =
        Assert.AreEqual(title, podcast.Title)
        Assert.AreEqual(description, podcast.Description)
        Assert.AreEqual(url, podcast.URL)
        Assert.AreEqual(imageURL, podcast.ImageURL)
        Assert.AreEqual(fileSize, podcast.FileDetails.FileSize)
        Assert.AreEqual(pubDate, podcast.PubDate)
        Assert.AreEqual(imageFileName, podcast.FileDetails.ImageFileName)
