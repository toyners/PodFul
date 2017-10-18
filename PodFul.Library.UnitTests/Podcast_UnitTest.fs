namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System

type Podcast_UnitTest() = 
    
    [<Test>]
    member public this.``Two podcast records are equal because of same title.``() =
        
        let podcast1 = Setup.createTestPodcast "title" "description1" "url1" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image1" "image1URL"
        let podcast2 = Setup.createTestPodcast "title" "description2" "url2" (new DateTime(2016, 12, 30)) 2L FeedFunctions.NoDateTime "image2" "image2URL"

        Assert.AreEqual(true, (podcast1 = podcast2))

    [<Test>]
    member public this.``Two podcast records are not equal because of different title.``() =
        
        let podcast1 = Setup.createTestPodcast "title1" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image" "imageURL"
        let podcast2 = Setup.createTestPodcast "title2" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image" "imageURL"

        Assert.AreEqual(false, (podcast1 = podcast2))

    [<Test>]
    member public this.``Setting file details updates the podcast file details``() =

        let imageFileName = "image"
        let podcast = Setup.createTestPodcast "title" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MinValue "" ""
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

        let podcast = Setup.createTestPodcast "title" "description" url (new DateTime(2016, 12, 31)) 1L FeedFunctions.NoDateTime "image" ""

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
    [<TestCase("http://abc.com/fileName.mp3?dest-id=92518")>]
    [<TestCase("http://abc.com/fileName")>]
    [<TestCase("http://abc.mp3.com/fileName")>]
    member public this.``Getting file name when URL is valid returns file name only``(url : string) =

        let podcast = Setup.createTestPodcast "title" "description" url (new DateTime(2016, 12, 31)) 1L FeedFunctions.NoDateTime "image" ""

        Assert.AreEqual("fileName.mp3", podcast.FileName)

    [<Test>]
    member public this.``Finalising file names using simple Finaliser with no clashes``() =

        let expectedFileName1 = "fileName1.mp3"
        let expectedFileName2 = "fileName2.mp3"
        let expectedFileName3 = "fileName3.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/fileName1.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/fileName2.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/fileName3.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
            |] 

        Assert.AreEqual(3, podcasts.Length)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)
