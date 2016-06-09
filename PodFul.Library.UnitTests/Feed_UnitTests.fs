namespace PodFul.Library.UnitTests

open FsUnit
open NUnit.Framework
open PodFul.Library
//open System

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
        