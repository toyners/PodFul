namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System

type Podcast_UnitTest() = 
    
    [<Test>]
    member public this.``Two podcast records are equal because of same title.``() =
        
        let podcast1 = Setup.createTestPodcast "title" "description1" "url1" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image1" "image1URL" String.Empty
        let podcast2 = Setup.createTestPodcast "title" "description2" "url2" (new DateTime(2016, 12, 30)) 2L FeedFunctions.NoDateTime "image2" "image2URL" String.Empty

        Assert.AreEqual(true, (podcast1 = podcast2))

    [<Test>]
    member public this.``Two podcast records are not equal because of different title.``() =
        
        let podcast1 = Setup.createTestPodcast "title1" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image" "imageURL" String.Empty
        let podcast2 = Setup.createTestPodcast "title2" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image" "imageURL" String.Empty

        Assert.AreEqual(false, (podcast1 = podcast2))

    [<Test>]
    member public this.``Setting file details updates the podcast file details``() =

        let imageFileName = "image"
        let podcast = Setup.createTestPodcast "title" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MinValue String.Empty String.Empty String.Empty
        let fileDetails = podcast.FileDetails

        podcast.SetAllFileDetails 2L DateTime.MaxValue imageFileName

        Assert.AreNotSame(fileDetails, podcast.FileDetails)
        Assert.AreEqual(2L, podcast.FileDetails.FileSize)
        Assert.AreEqual(DateTime.MaxValue, podcast.FileDetails.DownloadDate)
        Assert.AreEqual(imageFileName, podcast.FileDetails.ImageFileName)