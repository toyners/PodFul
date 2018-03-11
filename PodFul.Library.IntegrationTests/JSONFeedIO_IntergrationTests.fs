namespace PodFul.Library.IntegrationTests

open System
open System.IO
open System.Runtime.Serialization.Json
open System.Text
open Jabberwocky.Toolkit.IO
open PodFul.Library
open PodFul.TestSupport
open NUnit.Framework

type JSONFeedIO_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\JSONFeedIO_IntergrationTests\";

    let createPreviousTestFeed : PreviousFeed = 
        {
            Title = "title"
            Description = "description"
            Website = "website"
            Directory = "directory"
            URL = "url"
            ImageURL = "image URL"
            ImageFileName = "image file name"
            Podcasts = [||]
            CreationDateTime = new DateTime(2016, 2, 3)
            UpdatedDateTime = Miscellaneous.NoDateTime
            DoScan = true
            CompleteDownloadsOnScan = true
            DeliverDownloadsOnScan = true
        }

    let writePreviousTestFeedToFile (feed : PreviousFeed) (filePath : string) =
        use ms = new MemoryStream()
        (new DataContractJsonSerializer(typeof<PreviousFeed>)).WriteObject(ms, feed)
    
        use writer = new StreamWriter(filePath)
        writer.Write(Encoding.Default.GetString(ms.ToArray()))

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Reading Feed JSON file containing previous format returns latest Feed object``() = 
        let feedFilePath = workingDirectory + "testFeed.feed"
        let previousFeed = createPreviousTestFeed
        writePreviousTestFeedToFile previousFeed feedFilePath

        let feed = JSONFeedIO.ReadFeedFromFile(feedFilePath)

        Assert.AreEqual(previousFeed.Title, feed.Title)
        Assert.AreEqual(previousFeed.Description, feed.Description)
        Assert.AreEqual(previousFeed.Website, feed.Website)
        Assert.AreEqual(previousFeed.Directory, feed.Directory)
        Assert.AreEqual(previousFeed.URL, feed.URL)
        Assert.AreEqual(previousFeed.ImageURL, feed.ImageURL)
        Assert.AreEqual(previousFeed.ImageFileName, feed.ImageFileName)
        Assert.AreEqual(0, feed.Podcasts.Length)
        Assert.AreEqual(previousFeed.CreationDateTime, feed.CreationDateTime)
        Assert.AreEqual(previousFeed.UpdatedDateTime, feed.UpdatedDateTime)
        Assert.AreEqual(true, feed.DoScan)
        Assert.AreEqual(true, feed.CompleteDownloadsOnScan)
        Assert.AreEqual(true, feed.DeliverDownloadsOnScan)
        Assert.AreEqual(3, feed.ConfirmDownloadThreshold)

    [<Test>]
    member public this.``Reading Feed JSON file containing latest format returns latest Feed object``() = 
        let feedFilePath = workingDirectory + "testFeed.feed"
        let previousFeed = Setup.createDefaultTestFeed

        JSONFeedIO.WriteFeedToFile Setup.createDefaultTestFeed feedFilePath
        
        let feed = JSONFeedIO.ReadFeedFromFile(feedFilePath)

        Assert.AreEqual(previousFeed.Title, feed.Title)
        Assert.AreEqual(previousFeed.Description, feed.Description)
        Assert.AreEqual(previousFeed.Website, feed.Website)
        Assert.AreEqual(previousFeed.Directory, feed.Directory)
        Assert.AreEqual(previousFeed.URL, feed.URL)
        Assert.AreEqual(previousFeed.ImageURL, feed.ImageURL)
        Assert.AreEqual(previousFeed.ImageFileName, feed.ImageFileName)
        Assert.AreEqual(0, feed.Podcasts.Length)
        Assert.AreEqual(previousFeed.CreationDateTime, feed.CreationDateTime)
        Assert.AreEqual(previousFeed.UpdatedDateTime, feed.UpdatedDateTime)
        Assert.AreEqual(previousFeed.DoScan, feed.DoScan)
        Assert.AreEqual(previousFeed.CompleteDownloadsOnScan, feed.CompleteDownloadsOnScan)
        Assert.AreEqual(previousFeed.DeliverDownloadsOnScan, feed.DeliverDownloadsOnScan)

    [<Test>]
    member public this.``Reading empty Feed JSON file  throws exception as expected``() = 
        let feedFilePath = workingDirectory + "testFeed.feed"
        let mutable testSuccessful = false
        try
            JSONFeedIO.ReadFeedFromFile(feedFilePath) |> ignore
            testSuccessful <- false
        with
        | :? System.IO.FileNotFoundException as e ->
            testSuccessful <- true

        Assert.AreEqual(true, testSuccessful)