namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
open System

type Podcast_UnitTest() = 
    
    let createPodcastRecord title description url pubDate fileSize downloadDate imageFileName =
        {
            Title = title
            Description = description
            URL = url
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
        
        let podcast1 = createPodcastRecord "title" "description1" "url1" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image1"
        let podcast2 = createPodcastRecord "title" "description2" "url2" (new DateTime(2016, 12, 30)) 2L FeedFunctions.NoDateTime "image2"

        podcast1 = podcast2 |> should equal true

    [<Test>]
    member public this.``Two podcast records are not equal because of different title.``() =
        
        let podcast1 = createPodcastRecord "title1" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image"
        let podcast2 = createPodcastRecord "title2" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MaxValue "image"

        podcast1 = podcast2 |> should equal false

    [<Test>]
    member public this.``Setting file details updates the podcast file details``() =

        let podcast = createPodcastRecord "title" "description" "url" (new DateTime(2016, 12, 31)) 1L DateTime.MinValue ""
        let fileDetails = podcast.FileDetails

        podcast.SetAllFileDetails 2L DateTime.MaxValue "image"

        podcast.Title |> should equal podcast.Title
        podcast.Description |> should equal podcast.Description
        podcast.URL |> should equal podcast.URL
        podcast.PubDate |> should equal podcast.PubDate
        podcast.FileDetails |> should not' (be sameAs fileDetails)
        podcast.FileDetails.FileSize |> should equal 2L
        podcast.FileDetails.DownloadDate |> should equal DateTime.MaxValue
        podcast.FileDetails.ImageFileName |> should equal "image"

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Getting file name when URL is null or empty throws meaningful exception``(url : string) =

        let podcast = createPodcastRecord "title" "description" url (new DateTime(2016, 12, 31)) 1L FeedFunctions.NoDateTime "image"

        (fun() -> podcast.FileName |> ignore)
        |> should (throwWithMessage "Cannot get FileName: URL is null or empty.") typeof<System.Exception>

    [<Test>]
    [<TestCase("fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3")>]
    member public this.``Getting file name when URL is valid returns file name only``(url : string) =

        let podcast = createPodcastRecord "title" "description" url (new DateTime(2016, 12, 31)) 1L FeedFunctions.NoDateTime "image"

        podcast.FileName |> should equal "fileName.mp3"
