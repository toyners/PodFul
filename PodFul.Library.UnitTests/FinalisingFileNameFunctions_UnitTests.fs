namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System

type FinalisingFileNameFunctions_UnitTests() = 

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Finalising null or empty url results in default file name``(url : string) =

        let expectedFileName = "file_" + DateTime.Now.ToString("ddMMyyyy") + "_1.mp3"
        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL"
            ]

        let results = FinalisingFileNameFunctions.finaliseUsingStandardAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(1,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName, podcasts.[0].FileDetails.FileName)

    [<Test>]
    [<TestCase("fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3?dest-id=92518")>]
    [<TestCase("http://abc.com/fileName")>]
    [<TestCase("http://abc.mp3.com/fileName")>]
    member public this.``Finalising different url formats using simple finalising function``(url : string) =

        let expectedFileName = "fileName.mp3"
        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL"
            ]

        let results = FinalisingFileNameFunctions.finaliseUsingStandardAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(1,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName, podcasts.[0].FileDetails.FileName)

    [<Test>]
    member public this.``Finalising urls with no clashes using simple finalising function``() =

        let expectedFileName1 = "fileName1.mp3"
        let expectedFileName2 = "fileName2.mp3"
        let expectedFileName3 = "fileName3.mp3"

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" "http://abc.com/fileName1.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/fileName2.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/fileName3.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
            ]
            
        let results = FinalisingFileNameFunctions.finaliseUsingStandardAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(3,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)

    [<Test>]
    member public this.``Finalising urls with clashes using simple finalising function``() =

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
            ]
            
        let results = FinalisingFileNameFunctions.finaliseUsingStandardAlgorithm podcasts

        Assert.AreEqual(false, fst results)
