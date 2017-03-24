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

    let assertPodcastsMatch (podcast1 : Podcast) (podcast2 : Podcast) =
        Assert.AreEqual(podcast1.Title, podcast2.Title)
        Assert.AreEqual(podcast1.Description, podcast2.Description)
        Assert.AreEqual(podcast1.URL, podcast2.URL)
        Assert.AreEqual(podcast1.ImageURL, podcast2.ImageURL)
        Assert.AreEqual(podcast1.PubDate, podcast2.PubDate)
        Assert.AreEqual(podcast1.FileDetails.DownloadDate, podcast2.FileDetails.DownloadDate)
        Assert.AreEqual(podcast1.FileDetails.FileSize, podcast2.FileDetails.FileSize)
        Assert.AreEqual(podcast1.FileDetails.ImageFileName, podcast2.FileDetails.ImageFileName)
