namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
open System

type Feed_UnitTest() = 
    
    [<Test>]
    member public this.``Setting directory path returns new feed with correct directory path set``() =
        let feed1 =
            {
                Title = "title"
                Description = "description"
                Website = "website"
                Directory = "directory1"
                URL = "url"
                Podcasts = 
                [|
                    {
                        Title = "title"
                        Description = "description"
                        URL = "url"
                        FileSize = 1L
                        PubDate = DateTime.MinValue
                        DownloadDate = DateTime.MinValue
                    }
                |]
            }

        let feed2 = Feed.SetDirectory feed1 "directory2"

        feed2 |> should not' (be sameAs feed1)
        feed2.Title |> should equal feed1.Title
        feed2.Description |> should equal feed1.Description
        feed2.Website |> should equal feed1.Website
        feed2.Directory |> should equal "directory2"
        feed2.Podcasts |> should be (sameAs feed1.Podcasts)

