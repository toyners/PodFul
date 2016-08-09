﻿namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
open System

type Podcast_UnitTest() = 
    
    [<Test>]
    member public this.``Two podcast records are equal because of same title.``() =
        let podcast1 = 
            {
                Title = "title"
                Description = "description1"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = DateTime.MaxValue
                ImageFileName = "image1"
            }

        let podcast2 =
            {
                Title = "title"
                Description = "description2"
                URL = "url2"
                FileSize = 2L
                PubDate = new DateTime(2016, 12, 30)
                DownloadDate = FeedFunctions.NoDateTime
                ImageFileName = "image2"
            }

        podcast1 = podcast2 |> should equal true

    [<Test>]
    member public this.``Two podcast records are not equal because of different title.``() =
        let podcast1 = 
            {
                Title = "title1"
                Description = "description1"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = DateTime.MaxValue
                ImageFileName = "image"
            }

        let podcast2 =
            {
                Title = "title2"
                Description = "description2"
                URL = "url2"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = DateTime.MaxValue
                ImageFileName = "image"
            }

        podcast1 = podcast2 |> should equal false

    [<Test>]
    member public this.``Setting download date returns new record with download date set``() =
        let podcast1 =
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = FeedFunctions.NoDateTime
                ImageFileName = "image"
            }

        let podcast2 = Podcast.SetDownloadDate DateTime.MaxValue podcast1

        podcast2 |> should not' (be sameAs podcast1)
        podcast2.Title |> should equal podcast1.Title
        podcast2.Description |> should equal podcast1.Description
        podcast2.URL |> should equal podcast1.URL
        podcast2.FileSize |> should equal podcast1.FileSize
        podcast2.PubDate |> should equal podcast1.PubDate
        podcast2.DownloadDate |> should equal DateTime.MaxValue

    [<Test>]
    member public this.``Setting file size returns new record with file size set``() =
        let podcast1 =
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = FeedFunctions.NoDateTime
                ImageFileName = "image"
            }

        let podcast2 = Podcast.SetFileSize 2L podcast1

        podcast2 |> should not' (be sameAs podcast1)
        podcast2.Title |> should equal podcast1.Title
        podcast2.Description |> should equal podcast1.Description
        podcast2.URL |> should equal podcast1.URL
        podcast2.FileSize |> should equal 2L
        podcast2.PubDate |> should equal podcast1.PubDate
        podcast2.DownloadDate |> should equal podcast1.DownloadDate

    [<Test>]
    member public this.``Setting file image file name returns new record with image file name set``() =
        let podcast1 =
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = FeedFunctions.NoDateTime
                ImageFileName = ""
            }

        let podcast2 = Podcast.SetImageFileName "image" podcast1

        podcast2 |> should not' (be sameAs podcast1)
        podcast2.Title |> should equal podcast1.Title
        podcast2.Description |> should equal podcast1.Description
        podcast2.URL |> should equal podcast1.URL
        podcast2.FileSize |> should equal podcast1.FileSize
        podcast2.PubDate |> should equal podcast1.PubDate
        podcast2.DownloadDate |> should equal podcast1.DownloadDate
        podcast2.ImageFileName |> should equal "image"

    [<Test>]
    [<TestCase(null)>]
    [<TestCase("")>]
    member public this.``Getting file name when URL is null or empty throws meaningful exception``(url : string) =
        let podcast =
            {
                Title = "title"
                Description = "description"
                URL = url
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = FeedFunctions.NoDateTime
                ImageFileName = ""
            }

        (fun() -> podcast.FileName |> ignore)
        |> should (throwWithMessage "Cannot get FileName: URL is null or empty.") typeof<System.Exception>

    [<Test>]
    [<TestCase("fileName.mp3")>]
    [<TestCase("http://abc.com/fileName.mp3")>]
    [<TestCase(@"C:\abc\fileName.mp3")>]
    member public this.``Getting file name when URL is valid returns file name only``(url : string) =
        let podcast =
            {
                Title = "title"
                Description = "description"
                URL = url
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = FeedFunctions.NoDateTime
                ImageFileName = ""
            }

        podcast.FileName |> should equal "fileName.mp3"
