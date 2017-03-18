namespace PodFul.Library.UnitTests

open System
open NUnit.Framework
open PodFul.Library

type Feed_UnitTest() = 

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
            DoScan = true
            CompleteDownloadsOnScan = true
            DeliverDownloadsOnScan = true
        }

    [<Test>]
    member public this.``Setting file image file name returns new record with image file name set``() = 
        
        let feed1 = this.CreateFeed
        let feed2 = Feed.SetImageFileName feed1 "imagefilename"

        Assert.AreNotSame(feed1, feed2)
        Assert.AreEqual(feed1.Title, feed2.Title)
        Assert.AreEqual(feed1.Description, feed2.Description)
        Assert.AreEqual(feed1.Website, feed2.Website)
        Assert.AreEqual(feed1.Directory, feed2.Directory)
        Assert.AreEqual(feed1.URL, feed2.URL) 
        Assert.AreEqual("imagefilename", feed2.ImageFileName)
        Assert.AreEqual(feed1.Podcasts, feed2.Podcasts)
        Assert.AreEqual(feed1.CreationDateTime, feed2.CreationDateTime)
        Assert.AreEqual(feed1.UpdatedDateTime, feed2.UpdatedDateTime)

    [<Test>]
    member public this.``Setting updated date returns new record with updated date set``() = 
        let feed1 = this.CreateFeed
        let feed2 = Feed.SetUpdatedDate DateTime.MaxValue feed1

        Assert.AreNotSame(feed1, feed2)
        Assert.AreEqual(feed1.Title, feed2.Title)
        Assert.AreEqual(feed1.Description, feed2.Description)
        Assert.AreEqual(feed1.Website, feed2.Website)
        Assert.AreEqual(feed1.Directory, feed2.Directory)
        Assert.AreEqual(feed1.URL, feed2.URL) 
        Assert.AreEqual(feed1.ImageFileName, feed2.ImageFileName)
        Assert.AreEqual(feed1.Podcasts, feed2.Podcasts)
        Assert.AreEqual(feed1.CreationDateTime, feed2.CreationDateTime)
        Assert.AreEqual(DateTime.MaxValue, feed2.UpdatedDateTime)
        
    [<Test>]
    member public this.``Setting directory returns new record with updated directory set``() =
        let feed1 = this.CreateFeed
        let feed2 = Feed.SetDirectory "New Directory" feed1

        Assert.AreNotSame(feed1, feed2)
        Assert.AreEqual(feed1.Title, feed2.Title)
        Assert.AreEqual(feed1.Description, feed2.Description)
        Assert.AreEqual(feed1.Website, feed2.Website)
        Assert.AreEqual("New Directory", feed2.Directory)
        Assert.AreEqual(feed1.URL, feed2.URL) 
        Assert.AreEqual(feed1.ImageFileName, feed2.ImageFileName)
        Assert.AreEqual(feed1.Podcasts, feed2.Podcasts)
        Assert.AreEqual(feed1.CreationDateTime, feed2.CreationDateTime)
        Assert.AreEqual(feed1.UpdatedDateTime, feed2.UpdatedDateTime)