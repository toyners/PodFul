namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System

type FinalisingFileNameFunctions_UnitTests() = 

    let feedName = "feed name"

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Null or empty url results in file name using feed name and podcast title``(url : string) =

        let expectedFileName1 = "feed name - title1.mp3"
        let expectedFileName2 = "feed name - title2.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL" String.Empty
                Setup.createTestPodcast "title2" "description2" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image2" "imageURL" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(2,  Array.length <| results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Null or empty url results in file name using feed name and pubdate when there is a title clash``(url : string) =

        let pubDate = new DateTime(2017, 6, 5, 10, 3, 56)
        let expectedFileName1 = "feed name - title.mp3"
        let expectedFileName2 = "feed name - 05-06-2017 10-03-56.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL" String.Empty
                Setup.createTestPodcast "title" "description2" url pubDate 1L FeedFunctions.NoDateTime "image2" "imageURL" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(2,  Array.length <| results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Null or empty url results in empty file name when title and pubdate clash``(url : string) =

        let pubDate = new DateTime(2017, 6, 5, 10, 3, 56)
        let expectedFileName1 = "feed name - title.mp3"
        let expectedFileName2 = "feed name - 05-06-2017 10-03-56.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title" "description1" url pubDate 1L FeedFunctions.NoDateTime "image1" "imageURL" String.Empty
                Setup.createTestPodcast "title" "description2" url pubDate 1L FeedFunctions.NoDateTime "image2" "imageURL" String.Empty
                Setup.createTestPodcast "title" "description3" url pubDate 1L FeedFunctions.NoDateTime "image2" "imageURL" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(3,  Array.length <| results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(String.Empty, podcasts.[2].FileDetails.FileName)

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Null or empty url results in empty file name when title clash and pubdate is MinValue``(url : string) =
      
        let expectedFileName = "feed name - title.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL" String.Empty
                Setup.createTestPodcast "title" "description2" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image2" "imageURL" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(2,  Array.length <| results)
        Assert.AreEqual(expectedFileName, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(String.Empty, podcasts.[1].FileDetails.FileName)

    [<Test>]
    [<TestCase("fileName")>]
    [<TestCase("fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3")>]
    [<TestCase("http://abc.com/fileName")>]
    [<TestCase("http://abc.com/fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3?dest-id=92518")>]
    [<TestCase("http://abc.mp3.com/fileName")>]
    member public this.``Finalising different url examples using standard finalising function``(url : string) =

        let expectedFileName = "fileName.mp3"
        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" url FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL" String.Empty
            |]

        // Feed name is not important when the name can be resolved from the url
        let results = FinalisingFileNameFunctions.finaliseFileNames String.Empty podcasts

        Assert.AreEqual(1,  Array.length <| results)
        Assert.AreEqual(expectedFileName, podcasts.[0].FileDetails.FileName)

    [<Test>]
    member public this.``Finalising urls with no clashes using standard finalising function``() =

        let expectedFileName1 = "fileName1.mp3"
        let expectedFileName2 = "fileName2.mp3"
        let expectedFileName3 = "fileName3.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/fileName1.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1" String.Empty
                Setup.createTestPodcast "title2" "description2" "http://abc.com/fileName2.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2" String.Empty
                Setup.createTestPodcast "title3" "description3" "http://abc.com/fileName3.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
            |]
            
        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(3,  Array.length <| results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)

    [<Test>]
    member public this.``Finalising urls with all types of clashes``() =

        let pubDate = new DateTime(2017, 6, 5, 10, 3, 56)
        let expectedFileName1 = "fileName.mp3" // Name taken from the url
        let expectedFileName2 = "feed name - title2.mp3" // Name taken from feed title and podcast title
        let expectedFileName3 = "feed name - 05-06-2017 10-03-56.mp3" // Name taken from feed title and pubdate
        let expectedFileName4 = String.Empty // No unique name resolved - no name set

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1" String.Empty
                Setup.createTestPodcast "title2" "description2" "http://abc.com/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2" String.Empty
                Setup.createTestPodcast "title2" "description3" "http://abc.com/fileName.mp3" pubDate 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
                Setup.createTestPodcast "title2" "description3" "http://abc.com/fileName.mp3" pubDate 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
            |]
            
        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(4,  Array.length <| results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)
        Assert.AreEqual(expectedFileName4, podcasts.[3].FileDetails.FileName)

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
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1" String.Empty
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2" String.Empty
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode3/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
            |]
            
        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(3,  Array.length results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)

    [<Test>]
    member public this.``Alternate naming algorithm has most successful naming resolutions``() =
        
        let expectedFileName1 = "episode1.mp3"
        let expectedFileName2 = "episode2.mp3"
        let expectedFileName3 = "episode3.mp3"
        let expectedFileName4 = feedName + " - title4.mp3"
        let expectedFileName5 = feedName + " - title5.mp3"
        let expectedFileName6 = feedName + " - title6.mp3"
        let expectedFileName7 = String.Empty

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1" String.Empty
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2" String.Empty
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode3/fileName.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
                Setup.createTestPodcast "title4" "description4" "http://abc.com/episode1/fileName1.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title5" "description4" "http://abc.com/episode2/fileName1.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title6" "description4" String.Empty FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title6" "description4" String.Empty FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(7,  Array.length results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)
        Assert.AreEqual(expectedFileName4, podcasts.[3].FileDetails.FileName)
        Assert.AreEqual(expectedFileName5, podcasts.[4].FileDetails.FileName)
        Assert.AreEqual(expectedFileName6, podcasts.[5].FileDetails.FileName)
        Assert.AreEqual(expectedFileName7, podcasts.[6].FileDetails.FileName)

    [<Test>]
    member public this.``Standard naming algorithm has most successful naming resolutions``() =
        
        let expectedFileName1 = "fileName1.mp3"
        let expectedFileName2 = "fileName2.mp3"
        let expectedFileName3 = feedName + " - title3.mp3"
        let expectedFileName4 = "fileName3.mp3"
        let expectedFileName5 = "fileName4.mp3"
        let expectedFileName6 = feedName + " - title6.mp3"
        let expectedFileName7 = String.Empty

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName1.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1" String.Empty
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName2.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2" String.Empty
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode/fileName1.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
                Setup.createTestPodcast "title4" "description4" "http://abc.com/episode/fileName3.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title5" "description4" "http://abc.com/episode/fileName4.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title6" "description4" String.Empty FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title6" "description4" String.Empty FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(7,  Array.length results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)
        Assert.AreEqual(expectedFileName4, podcasts.[3].FileDetails.FileName)
        Assert.AreEqual(expectedFileName5, podcasts.[4].FileDetails.FileName)
        Assert.AreEqual(expectedFileName6, podcasts.[5].FileDetails.FileName)
        Assert.AreEqual(expectedFileName7, podcasts.[6].FileDetails.FileName)

    [<Test>]
    member public this.``Standard naming algorithm has the same number of resolutions as alternative naming algorithm``() =
        
        let expectedFileName1 = "fileName1.mp3"
        let expectedFileName2 = "fileName2.mp3"
        let expectedFileName3 = "fileName3.mp3"
        let expectedFileName4 = feedName + " - title4.mp3"
        let expectedFileName5 = feedName + " - title5.mp3"
        let expectedFileName6 = feedName + " - title6.mp3"
        let expectedFileName7 = feedName + " - title7.mp3"

        let podcasts =
            [|
                Setup.createTestPodcast "title1" "description1" "http://abc.com/episode1/fileName1.mp3" FeedFunctions.NoDateTime 1L FeedFunctions.NoDateTime "image1" "imageURL1" String.Empty
                Setup.createTestPodcast "title2" "description2" "http://abc.com/episode2/fileName2.mp3" FeedFunctions.NoDateTime 2L FeedFunctions.NoDateTime "image2" "imageURL2" String.Empty
                Setup.createTestPodcast "title3" "description3" "http://abc.com/episode3/fileName3.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL3" String.Empty
                Setup.createTestPodcast "title4" "description4" "http://abc.com/episode1/fileName1.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title5" "description5" "http://abc.com/episode2/fileName2.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title6" "description6" "http://abc.com/episode3/fileName3.mp3" FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
                Setup.createTestPodcast "title7" "description7" String.Empty FeedFunctions.NoDateTime 3L FeedFunctions.NoDateTime "image3" "imageURL4" String.Empty
            |]

        let results = FinalisingFileNameFunctions.finaliseFileNames feedName podcasts

        Assert.AreEqual(7,  Array.length results)
        Assert.AreEqual(expectedFileName1, podcasts.[0].FileDetails.FileName)
        Assert.AreEqual(expectedFileName2, podcasts.[1].FileDetails.FileName)
        Assert.AreEqual(expectedFileName3, podcasts.[2].FileDetails.FileName)
        Assert.AreEqual(expectedFileName4, podcasts.[3].FileDetails.FileName)
        Assert.AreEqual(expectedFileName5, podcasts.[4].FileDetails.FileName)
        Assert.AreEqual(expectedFileName6, podcasts.[5].FileDetails.FileName)
        Assert.AreEqual(expectedFileName7, podcasts.[6].FileDetails.FileName)
