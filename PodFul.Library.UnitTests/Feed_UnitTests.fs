namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library

type Feed_UnitTest() = 

    [<Test>]
    member public this.``Setting file image file name returns new record with image file name set``() = 
        let feed1 = 
            {
                Title = "title"
                Description = "description"
                Website = "website"
                Directory = "directory"
                URL = "url"
                ImageFileName = ""
                Podcasts = null
            }

        let feed2 = Feed.SetImageFileName feed1 "imagefilename"

        feed2 |> should not' (be sameAs feed1)
        feed2.Title |> should equal feed1.Title
        feed2.Description |> should equal feed1.Description
        feed2.Website |> should equal feed1.Website
        feed2.Directory |> should equal feed1.Directory
        feed2.URL |> should equal feed1.URL
        feed2.ImageFileName |> should equal "imagefilename"
        feed2.Podcasts |> should equal feed1.Podcasts
        