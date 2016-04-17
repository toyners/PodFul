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
            }

        let podcast2 =
            {
                Title = "title2"
                Description = "description2"
                URL = "url"
                FileSize = 2L
                PubDate = new DateTime(2016, 12, 30)
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
            }

        let podcast2 =
            {
                Title = "title"
                Description = "description"
                URL = "url2"
                FileSize = 1L
                PubDate = new DateTime(2016, 12, 31)
            }

        podcast1 = podcast2 |> should equal false