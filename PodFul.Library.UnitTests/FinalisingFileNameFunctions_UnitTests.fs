namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport

type FinalisingFileNameFunctions_UnitTests() = 

    [<Test>]
    member public this.``Finalising file names using simple Finaliser with no clashes``() =

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


