namespace PodFul.Library.UnitTests

open System
open FsUnit
open NUnit.Framework
open PodFul.Library

(*type Feed_UnitTest() = 
    none
    member private this.CreateFeed =
        {
            Title = "title"
            Description = "description"
            Website = "website"
            Directory = "directory"
            URL = "url"
            ImageURL = "image URL"
            ImageFileName = ""
            Podcasts = null
            CreationDateTime = new DateTime(2016, 2, 3)
            UpdatedDateTime = FeedFunctions.NoDateTime
        }

    [<Test>]
    member public this.``Setting file image file name returns new record with image file name set``() = 
        
        let feed1 = this.CreateFeed
        let feed2 = Feed.SetImageFileName feed1 "imagefilename"

        feed2 |> should not' (be sameAs feed1)
        feed2.Title |> should equal feed1.Title
        feed2.Description |> should equal feed1.Description
        feed2.Website |> should equal feed1.Website
        feed2.Directory |> should equal feed1.Directory
        feed2.URL |> should equal feed1.URL
        feed2.ImageFileName |> should equal "imagefilename"
        feed2.Podcasts |> should equal feed1.Podcasts
        feed2.CreationDateTime |> should equal feed1.CreationDateTime
        feed2.UpdatedDateTime |> should equal feed1.UpdatedDateTime

    [<Test>]
    member public this.``Setting updated date returns new record with updated date set``() = 
        let feed1 = this.CreateFeed
        let feed2 = Feed.SetUpdatedDate DateTime.MaxValue feed1

        feed2 |> should not' (be sameAs feed1)
        feed2.Title |> should equal feed1.Title
        feed2.Description |> should equal feed1.Description
        feed2.Website |> should equal feed1.Website
        feed2.Directory |> should equal feed1.Directory
        feed2.URL |> should equal feed1.URL
        feed2.ImageFileName |> should equal feed1.ImageFileName
        feed2.Podcasts |> should equal feed1.Podcasts
        feed2.CreationDateTime |> should equal feed1.CreationDateTime
        feed2.UpdatedDateTime |> should equal DateTime.MaxValue
        *)