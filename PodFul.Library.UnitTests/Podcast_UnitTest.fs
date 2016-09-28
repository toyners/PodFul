namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open System

type Podcast_UnitTest() = 
    
    let createPodcastRecord title description url pubDate fileSize downloadDate imageFileName imageURL =
        {
            Title = title
            Description = description
            URL = url
            ImageURL = imageURL
            PubDate = pubDate
            FileDetails =
                {
                    FileSize = fileSize
                    DownloadDate = downloadDate
                    ImageFileName = imageFileName
                }
            
        }

    [<Test>]
    member public this.``Two podcast records are equal because of same title.``() =
        
        let podcast1 = createPodcastRecord "title" "description1" "url1" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image1" "image1URL"
        let podcast2 = createPodcastRecord "title" "description2" "url2" (new DateTime(2016, 12, 30)) 2L FeedFunctions.NoDateTime "image2" "image2URL"

        Assert.AreEqual(true, (podcast1 = podcast2))

    [<Test>]
    member public this.``Two podcast records are not equal because of different title.``() =
        
        let podcast1 = createPodcastRecord "title1" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image" "imageURL"
        let podcast2 = createPodcastRecord "title2" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image" "imageURL"

        Assert.AreEqual(false, (podcast1 = podcast2))

    [<Test>]
    member public this.``Setting file details updates the podcast file details``() =

        let imageFileName = "image"
        let podcast = createPodcastRecord "title" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MinValue "" ""
        let fileDetails = podcast.FileDetails

        podcast.SetAllFileDetails 2L DateTime.MaxValue imageFileName

        Assert.AreNotSame(fileDetails, podcast.FileDetails)
        Assert.AreEqual(2L, podcast.FileDetails.FileSize)
        Assert.AreEqual(DateTime.MaxValue, podcast.FileDetails.DownloadDate)
        Assert.AreEqual(imageFileName, podcast.FileDetails.ImageFileName)

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Getting file name when URL is null or empty throws meaningful exception``(url : string) =

        let podcast = createPodcastRecord "title" "description" url (new DateTime(2016, 12, 31)) 1L FeedFunctions.NoDateTime "image" ""

        let mutable testSuccessful = false
        try
            podcast.FileName |> ignore
        with
        | _ as e ->
            Assert.AreEqual("Cannot get FileName: URL is null or empty.", e.Message)
            testSuccessful <- true

        Assert.AreEqual(true, testSuccessful)

    [<Test>]
    [<TestCase("fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3")>]
    member public this.``Getting file name when URL is valid returns file name only``(url : string) =

        let podcast = createPodcastRecord "title" "description" url (new DateTime(2016, 12, 31)) 1L FeedFunctions.NoDateTime "image" ""

        Assert.AreEqual("fileName.mp3", podcast.FileName)
