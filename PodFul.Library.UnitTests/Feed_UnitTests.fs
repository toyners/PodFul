namespace PodFul.Library.UnitTests

open System
open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport

type Feed_UnitTest() = 

    [<Test>]
    member public this.``Setting file image file name returns new record with image file name set``() = 
        
        let feed1 = Setup.createDefaultTestFeed
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

        let feed1 = Setup.createDefaultTestFeed
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
    
        let feed1 = Setup.createDefaultTestFeed
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

    [<Test>]
    member public this.``Setting scanning flags returns new record with updated scanning flags set``() =
    
        let feed1 = Setup.createDefaultTestFeed
        let changedDoScanValue = not feed1.DoScan 
        let changedCompleteDownloadsOnScan = not feed1.CompleteDownloadsOnScan
        let changedDeliverDownloadsOnScan = not feed1.DeliverDownloadsOnScan
        let feed2 = Feed.SetScanningFlags changedDoScanValue false false feed1

        Assert.AreNotSame(feed1, feed2)
        Assert.AreEqual(feed1.Title, feed2.Title)
        Assert.AreEqual(feed1.Description, feed2.Description)
        Assert.AreEqual(feed1.Website, feed2.Website)
        Assert.AreEqual(feed1.Directory, feed2.Directory)
        Assert.AreEqual(feed1.URL, feed2.URL) 
        Assert.AreEqual(feed1.ImageFileName, feed2.ImageFileName)
        Assert.AreEqual(feed1.Podcasts, feed2.Podcasts)
        Assert.AreEqual(feed1.CreationDateTime, feed2.CreationDateTime)
        Assert.AreEqual(feed1.UpdatedDateTime, feed2.UpdatedDateTime)
        Assert.AreEqual(changedDoScanValue, feed2.DoScan)
        Assert.AreEqual(changedCompleteDownloadsOnScan, feed2.CompleteDownloadsOnScan)
        Assert.AreEqual(changedDeliverDownloadsOnScan, feed2.DeliverDownloadsOnScan)