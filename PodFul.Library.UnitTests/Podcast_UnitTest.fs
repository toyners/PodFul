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
                DownloadDate = DateTime.MaxValue
            }

        let podcast2 =
            {
                Title = "title2"
                Description = "description2"
                URL = "url"
                FileSize = 2L
                PubDate = new DateTime(2016, 12, 30)
                DownloadDate = DateTime.MinValue
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
                DownloadDate = DateTime.MaxValue
            }

        let podcast2 =
            {
                Title = "title"
                Description = "description"
                URL = "url2"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
                DownloadDate = DateTime.MaxValue
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
                DownloadDate = DateTime.MinValue
            }

        let podcast2 = Podcast.SetDownloadDate podcast1 DateTime.MaxValue

        podcast2 |> should not' (be sameAs podcast1)
        podcast2.Title |> should equal podcast1.Title
        podcast2.Description |> should equal podcast1.Description
        podcast2.URL |> should equal podcast1.URL
        podcast2.FileSize |> should equal podcast1.FileSize
        podcast2.PubDate |> should equal podcast1.PubDate
        podcast2.DownloadDate |> should equal DateTime.MaxValue