namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.Library
open PodFul.WPF.Miscellaneous

type TestStorage(feeds) = 

    interface IFeedStorage with
        
        member this.Feeds with get() = feeds
        member this.IsOpen with get() = true
        member this.Add (feed : Feed) = ()
        member this.Close() = ()
        member this.Open () = ()
        member this.Remove (feed : Feed) = ()
        member this.Update (feed : Feed) = ()

/// <summary>
/// Tests the different scenarios for crearting delivery point instances.
/// </summary>
type FeedCollections_UnitTests() =

    let createTestFeed =
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
    member public this.``Passing feed storage with no feeds throws meaningful exception``() =
        let mutable testPassed = false
        let testStorage = new TestStorage(null)
        
        try
            let feedCollection = new FeedCollection(testStorage)
            testPassed <- false
        with
            | e ->
                Assert.AreEqual("Feed Storage contains no feeds", e.Message)
                testPassed <- true
                
        Assert.AreEqual(testPassed, true)

    [<Test>]
    member public this.``Passing feed storage with zero feeds throws meaningful exception``() =
        let mutable testPassed = false
        let testStorage = new TestStorage([||])
        
        try
            let feedCollection = new FeedCollection(testStorage)
            testPassed <- false
        with
            | e ->
                Assert.AreEqual("Feed Storage contains no feeds", e.Message)
                testPassed <- true
                
        Assert.AreEqual(testPassed, true)

    [<Test>]
    [<TestCase(-1)>]
    [<TestCase(1)>]
    member public this.``Getting feed using out-of-range index throws meaningful exception``(index : int) =

        let mutable testPassed = false
        let expectedMessage = "Cannot get feed: Index (" + index.ToString() + ") is outside range (0..0)"
        let feeds = [| createTestFeed |]
        let testStorage = new TestStorage(feeds)
        let feedCollection = new FeedCollection(testStorage)
        
        try
            let feed = feedCollection.[index]
            testPassed <- false
        with
            | e ->
                Assert.AreEqual(expectedMessage, e.Message)
                testPassed <- true
                
        Assert.AreEqual(testPassed, true)

    [<Test>]
    [<TestCase(-1)>]
    [<TestCase(1)>]
    member public this.``Setting feed using out-of-range index throws meaningful exception``(index : int) =

        let mutable testPassed = false
        let expectedMessage = "Cannot set feed: Index (" + index.ToString() + ") is outside range (0..0)"
        let feed = createTestFeed
        let feeds = [| feed |]
        let testStorage = new TestStorage(feeds)
        let feedCollection = new FeedCollection(testStorage)
        
        try
            feedCollection.[index] <- feed
            testPassed <- false
        with
            | e ->
                Assert.AreEqual(expectedMessage, e.Message)
                testPassed <- true
                
        Assert.AreEqual(testPassed, true)