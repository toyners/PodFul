namespace PodFul.TestSupport

open PodFul.Library
open NUnit.Framework

module public Asserting = 

    let assertPodcastIsCorrect (podcast : Podcast) title description url imageURL fileSize pubDate imageFileName =
        Assert.AreEqual(title, podcast.Title)
        Assert.AreEqual(description, podcast.Description)
        Assert.AreEqual(url, podcast.URL)
        Assert.AreEqual(imageURL, podcast.ImageURL)
        Assert.AreEqual(fileSize, podcast.FileDetails.FileSize)
        Assert.AreEqual(pubDate, podcast.PubDate)
        Assert.AreEqual(imageFileName, podcast.FileDetails.ImageFileName)
