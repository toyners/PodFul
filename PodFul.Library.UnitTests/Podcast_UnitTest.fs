namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
open System

type Podcast_UnitTest() = 
    
    [<Test>]
    member public this.``Two different podcast records are equal because of same URL.``() =
        let podcast1 = 
            {
                Title = "title1"
                Description = "description1"
                URL = "url"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                FirstDownloadDate = DateTime.MinValue
                LatestDownloadDate = DateTime.MaxValue
            }

        let podcast2 =
            {
                Title = "title2"
                Description = "description2"
                URL = "url"
                FileSize = 2L
                PubDate = new DateTime(2016, 12, 30)
                FirstDownloadDate = DateTime.MaxValue
                LatestDownloadDate = DateTime.MinValue
            }

        podcast1 = podcast2 |> should equal true

    [<Test>]
    member public this.``Two different podcast records are not equal because of different URL.``() =
        let podcast1 = 
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                FirstDownloadDate = DateTime.MinValue
                LatestDownloadDate = DateTime.MaxValue
            }

        let podcast2 =
            {
                Title = "title"
                Description = "description"
                URL = "url2"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                FirstDownloadDate = DateTime.MinValue
                LatestDownloadDate = DateTime.MaxValue
            }

        podcast1 = podcast2 |> should equal false

    [<Test>]
    member public this.``Setting first download date returns new record with first download and latest download date set``() =
        let firstDownloadDate = new DateTime(2017, 1, 1)
        let podcast1 =
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                FirstDownloadDate = DateTime.MinValue
                LatestDownloadDate = DateTime.MaxValue
            }

        let podcast2 = Podcast.SetFirstDownloadDate podcast1 firstDownloadDate

        podcast2 |> should not' (be sameAs podcast1)
        podcast2.Title |> should equal podcast1.Title
        podcast2.Description |> should equal podcast1.Description
        podcast2.URL |> should equal podcast1.URL
        podcast2.FileSize |> should equal podcast1.FileSize
        podcast2.PubDate |> should equal podcast1.PubDate
        podcast2.FirstDownloadDate |> should equal firstDownloadDate
        podcast2.LatestDownloadDate |> should equal firstDownloadDate

    [<Test>]
    member public this.``Setting latest download date returns new record with latest download date set``() =
        let podcast1 =
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                FirstDownloadDate = DateTime.MinValue
                LatestDownloadDate = new DateTime(2017, 1, 2, 10, 11, 12)
            }

        let podcast2 = Podcast.SetLatestDownloadDate podcast1 DateTime.MaxValue

        podcast2 |> should not' (be sameAs podcast1)
        podcast2.Title |> should equal podcast1.Title
        podcast2.Description |> should equal podcast1.Description
        podcast2.URL |> should equal podcast1.URL
        podcast2.FileSize |> should equal podcast1.FileSize
        podcast2.PubDate |> should equal podcast1.PubDate
        podcast2.FirstDownloadDate |> should equal DateTime.MinValue
        podcast2.LatestDownloadDate |> should equal DateTime.MaxValue

    [<Test>]
    member public this.``Setting first download date again returns unchanged record``() =
        let podcast1 =
            {
                Title = "title"
                Description = "description"
                URL = "url1"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                FirstDownloadDate = new DateTime(2017, 1, 2, 10, 11, 12)
                LatestDownloadDate = new DateTime(2017, 1, 2, 10, 11, 12)
            }

        let podcast2 = Podcast.SetFirstDownloadDate podcast1 DateTime.MaxValue

        podcast2 |> should be (sameAs podcast1)
