namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System

type FinalisingFileNameFunctions_UnitTests() = 

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Finalising null or empty url results in default file name from standard finalising function``(url : string) =

        let expectedFileName1 = "file_" + DateTime.Now.ToString("ddMMyyyy") + "_1.mp3"
        let expectedFileName2 = "file_" + DateTime.Now.ToString("ddMMyyyy") + "_2.mp3"
        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL"
                Setup.createTestPodcast "title2" "description2" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image2" "imageURL"
            ]

        let results = FinalisingFileNameFunctions.finaliseUsingStandardAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(2,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)

    [<Test>]
    [<TestCase("fileName")>]
    [<TestCase("fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3")>]
    [<TestCase("http://abc.com/fileName")>]
    [<TestCase("http://abc.com/fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3?dest-id=92518")>]
    [<TestCase("http://abc.mp3.com/fileName")>]
    member public this.``Finalising different url formats using standard finalising function``(url : string) =

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
    member public this.``Finalising urls with no clashes using standard finalising function``() =

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
    member public this.``Finalising urls with clashes using standard finalising function``() =

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
            ]
            
        let results = FinalisingFileNameFunctions.finaliseUsingStandardAlgorithm podcasts

        Assert.AreEqual(false, fst results)

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Finalising null or empty url results in default file name from alternate finalising function``(url : string) =

        let expectedFileName1 = "file_" + DateTime.Now.ToString("ddMMyyyy") + "_1.mp3"
        let expectedFileName2 = "file_" + DateTime.Now.ToString("ddMMyyyy") + "_2.mp3"
        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL"
                Setup.createTestPodcast "title2" "description2" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image2" "imageURL"
            ]

        let results = FinalisingFileNameFunctions.finaliseUsingAlternateAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(2,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)

    [<Test>]
    member public this.``Finalising urls with clashes using alternate finalising function``() =

        let expectedFileName1 = "episode1.mp3"
        let expectedFileName2 = "episode2.mp3"
        let expectedFileName3 = "episode3.mp3"

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode3/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
            ]
            
        let results = FinalisingFileNameFunctions.finaliseUsingAlternateAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(3,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)

    [<Test>]
    [<TestCase("fileName", "fileName.mp3")>]
    [<TestCase("fileName.mp3", "fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3", "abc.mp3")>]
    [<TestCase(@"C:/abc/fileName.mp3", "abc.mp3")>]
    [<TestCase(@"C:\abc\def\fileName.mp3", "def.mp3")>]
    [<TestCase(@"C:/abc/def/fileName.mp3", "def.mp3")>]
    [<TestCase("http://abc.com/fileName", "abc.com.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3", "abc.com.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3?dest-id=92518", "abc.com.mp3")>]
    [<TestCase("http://abc.mp3.com/fileName", "abc.mp3")>]
    member public this.``Finalising different url formats using alternate finalising function``(url : string, expectedFileName : string) =

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL"
            ]

        let results = FinalisingFileNameFunctions.finaliseUsingAlternateAlgorithm podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(1,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName, podcasts.[0].FileDetails.FileName)

    [<Test>]
    [<TestCase("file:Name.mp3", "file-c-Name.mp3")>]
    [<TestCase("file*Name.mp3", "file-a-Name.mp3")>]
    [<TestCase("file?Name.mp3", "file-q-Name.mp3")>]
    [<TestCase("file\"Name.mp3", "file-qu-Name.mp3")>]
    [<TestCase("file<Name.mp3", "file-lt-Name.mp3")>]
    [<TestCase("file>Name.mp3", "file-gt-Name.mp3")>]
    [<TestCase("file|Name.mp3", "file-b-Name.mp3")>]
    member public this.``Substitute bad file name characters``(fileName : string, expectedFileName : string) =

        let actualFileName = FinalisingFileNameFunctions.substituteBadFileNameCharacters fileName

        Assert.AreEqual(expectedFileName, actualFileName)

    [<Test>]
    member public this.``Finalising urls with clashes in standard algorithm``() =

        let expectedFileName1 = "episode1.mp3"
        let expectedFileName2 = "episode2.mp3"
        let expectedFileName3 = "episode3.mp3"

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode3/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
            ]
            
        let results = FinalisingFileNameFunctions.finaliseFileNames "feed name" podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(3,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)

    member public this.``Finalising urls with clashes in standard and alternate algorithms``() =

        let expectedFileName1 = "feed name episode 1.mp3"
        let expectedFileName2 = "feed name episode 2.mp3"
        let expectedFileName3 = "feed name episode 3.mp3"
        let expectedFileName4 = "feed name episode 4.mp3"

        let podcasts =
            [
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1"
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2"
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3"
                Setup.createTestPodcast "title4" "description4" "http://abc.com/episode/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4"
            ]
            
        let results = FinalisingFileNameFunctions.finaliseFileNames "feed name" podcasts

        Assert.AreEqual(true, fst results)
        Assert.AreEqual(4,  List.length <| (snd results))
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)
        Assert.AreEqual(expectedFileName4, podcasts.[3].FileDetails.FileName)
