namespace PodFul.Library.IntegrationTests

open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open System
open System.IO
open System.Reflection
open System.Threading

type FeedFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\FeedFunctions_IntergrationTests\";

    let feedFileName = "Feed.rss"
    let rssFileName = "RSSFile.rss"
    let initialRSSFileName = "Initial RSSFile.rss"
    let finalRSSFileName = "Final RSSFile.rss"
    let initialRSSWithMaximumPodcastsFileName = "Initial RSSFile with Maximum Podcasts.rss"
    let finalRSSWithMaximumPodcastsFileName = "Final RSSFile with Maximum Podcasts.rss"
    let rssWithNoPodcasts = "No Podcasts.rss"
    let rssWithOnePodcast = "One Podcast.rss"
    let rssWithValidImages = "RSS with valid Images.rss"
    let itemWithMultipleEnclosureTags = "Item with Multiple Enclosure Tags.rss"
    let itemWithMultipleDuplicateEnclosureTags = "Item with Multiple Duplicate Enclosure Tags.rss"
    let itemWithMultipleMediaContentTags = "Item with Multiple MediaContent Tags.rss"
    let itemWithMultipleDuplicateMediaContentTags = "Item with Multiple Duplicate MediaContent Tags.rss"
    let itemWithEnclosureAndMediaContentTags = "Item with Enclosure and MediaContent Tags.rss"
    let itemWithDuplicateEnclosureAndMediaContentTags = "Item with Duplicate Enclosure and MediaContent Tags.rss"

    let feedTitle = "Feed Title"
    let feedDescription = "Feed Description"
    let feedWebsite = "Feed Website"
    let feedImageFileName = "Feed Image"

    let firstPodcastTitle = "Podcast #1 Title"
    let firstPodcastDescription = "Podcast #1 Description"
    let firstPodcastURL = "Podcast1.mp3"
    let firstPodcastFileSize = 1L
    let firstPodcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)
    let firstPodcastImageURL = "Podcast #1 Image"

    let secondPodcastTitle = "Podcast #2 Title"
    let secondPodcastDescription = "Podcast #2 Description"
    let secondPodcastURL = "Podcast2.mp3"
    let secondPodcastFileSize = 2L
    let secondPodcastPubDate = new DateTime(2015, 3, 4, 10, 11, 12)

    let thirdPodcastTitle = "Podcast #3 Title"
    let thirdPodcastURL = "Podcast3.mp3"
    let thirdPodcastFileSize = 3L

    let podcastTitle = "Podcast Title"
    let podcastDescription = "Podcast Description"
    let podcastImageURL = "Podcast Image"
    let podcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)

    let podcastTitle1of2 = "Podcast Title [1 of 2]"
    let podcastTitle2of2 = "Podcast Title [2 of 2]"

    let podcastTitle1of3 = "Podcast Title [1 of 3]"
    let podcastTitle2of3 = "Podcast Title [2 of 3]"
    let podcastTitle3of3 = "Podcast Title [3 of 3]"

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Create Feed from RSS url``() =
        let inputPath = workingDirectory + rssFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)
        let feed = Setup.createTestFeedWithDirectoryPath inputPath "DirectoryPath"

        Assert.AreNotEqual(null, feed)
        Assert.AreEqual(feedTitle, feed.Title)
        Assert.AreEqual(feedDescription, feed.Description)
        Assert.AreEqual(feedWebsite, feed.Website)
        Assert.AreEqual("DirectoryPath", feed.Directory)
        Assert.AreEqual(inputPath, feed.URL)
        Assert.AreEqual(feedImageFileName, feed.ImageURL)
        Assert.AreEqual("", feed.ImageFileName)

        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(2, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect 
            feed.Podcasts.[0] 
            firstPodcastTitle firstPodcastDescription firstPodcastURL firstPodcastImageURL
            firstPodcastFileSize firstPodcastPubDate String.Empty

        Asserting.assertPodcastIsCorrect 
            feed.Podcasts.[1] 
            secondPodcastTitle secondPodcastDescription secondPodcastURL String.Empty
            secondPodcastFileSize secondPodcastPubDate String.Empty     

    [<Test>]
    member public this.``Create Feed with download file parameter creates download file``() =
        let inputPath = workingDirectory + rssFileName
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)

        let feedFilePath = workingDirectory + "download.rss"
        Setup.createTestFeedUsingDownloadFile inputPath feedFilePath |> ignore

        let fileExists = System.IO.File.Exists(feedFilePath)

        Assert.AreEqual(true, fileExists)

    [<Test>]
    member public this.``Create Feed that contains mixture of media content and enclosure tags``() =
        let MultipleItemsWithEnclosureAndMediaContentTags = "Multiple Items with Enclosure and MediaContent Tags.rss"
        let inputPath = workingDirectory + MultipleItemsWithEnclosureAndMediaContentTags;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(MultipleItemsWithEnclosureAndMediaContentTags, inputPath)
        let feed = Setup.createTestFeedWithDirectoryPath inputPath "DirectoryPath"

        Assert.AreNotEqual(null, feed)
        Assert.AreEqual(feedTitle, feed.Title)
        Assert.AreEqual(feedDescription, feed.Description)
        Assert.AreEqual(feedWebsite, feed.Website)
        Assert.AreEqual("DirectoryPath", feed.Directory)
        Assert.AreEqual(inputPath, feed.URL)

        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(3, feed.Podcasts.Length)

        Assert.AreEqual(firstPodcastTitle, feed.Podcasts.[0].Title)
        Assert.AreEqual(firstPodcastURL, feed.Podcasts.[0].URL)
        Assert.AreEqual(firstPodcastFileSize, feed.Podcasts.[0].FileDetails.FileSize)

        Assert.AreEqual(secondPodcastTitle, feed.Podcasts.[1].Title)
        Assert.AreEqual(secondPodcastURL, feed.Podcasts.[1].URL)
        Assert.AreEqual(secondPodcastFileSize, feed.Podcasts.[1].FileDetails.FileSize)

        Assert.AreEqual(thirdPodcastTitle, feed.Podcasts.[2].Title)
        Assert.AreEqual(thirdPodcastURL, feed.Podcasts.[2].URL)
        Assert.AreEqual(thirdPodcastFileSize, feed.Podcasts.[2].FileDetails.FileSize)

    [<Test>]
    [<TestCase("RSSFile with Missing Image.rss")>]
    [<TestCase("RSSFile with Empty Image 1.rss")>]
    [<TestCase("RSSFile with Empty Image 2.rss")>]
    [<TestCase("RSSFile with Empty URL 1.rss")>]
    [<TestCase("RSSFile with Empty URL 2.rss")>]
    member public this.``Create feed from RSS file with no feed image data``(feedFileName : string) =
        let inputPath = workingDirectory + feedFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreEqual(String.Empty, feed.ImageURL)

    [<Test>]
    [<TestCase("RSS with valid Images.rss")>]
    [<TestCase("Feed with valid Image Attribute.rss")>]
    member public this.``Create feed from RSS file with feed image data``(feedFileName : string) =
        let inputPath = workingDirectory + feedFileName;
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreEqual("C:\Projects\PodFul\PodFul.Library.IntegrationTests\FeedFunctions_IntergrationTests\FeedImage.jpg", feed.ImageURL)

    [<Test>]
    member public this.``Update feed from RSS file``() =
        let initialInputPath = workingDirectory + initialRSSFileName
        let firstDateTime = new DateTime(2015, 1, 2, 8, 9, 10)
        let secondDateTime = new DateTime(2016, 3, 4, 11, 12, 13)

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSFileName, initialInputPath)
        let initialFeed = Setup.createTestFeed initialInputPath
        
        let firstPodcast = initialFeed.Podcasts.[0]
        firstPodcast.SetFileDetails firstPodcast.FileDetails.FileSize firstDateTime
        
        let secondPodcast = initialFeed.Podcasts.[1]
        secondPodcast.SetFileDetails secondPodcast.FileDetails.FileSize secondDateTime

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSFileName, initialInputPath)
        let finalFeed = Setup.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(3, finalFeed.Podcasts.Length)

        Assert.AreEqual(firstPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[1].FileDetails.DownloadDate)
        Assert.AreEqual(secondPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[2].FileDetails.DownloadDate)

    [<Test>]
    member public this.``Update feed from RSS file preserves scanning flags``() =
        let initialInputPath = workingDirectory + initialRSSFileName
        
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSFileName, initialInputPath)
        let initialFeed = Setup.createTestFeed initialInputPath
        
        let changedDoScanValue = not initialFeed.DoScan 
        let changedCompleteDownloadsOnScan = not initialFeed.CompleteDownloadsOnScan
        let changedDeliverDownloadsOnScan = not initialFeed.DeliverDownloadsOnScan
        let initialFeed = Feed.SetScanningFlags changedDoScanValue changedCompleteDownloadsOnScan changedDeliverDownloadsOnScan initialFeed
        
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSFileName, initialInputPath)
        let finalFeed = Setup.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed.DoScan, finalFeed.DoScan)
        Assert.AreEqual(initialFeed.CompleteDownloadsOnScan, finalFeed.CompleteDownloadsOnScan)
        Assert.AreEqual(initialFeed.DeliverDownloadsOnScan, finalFeed.DeliverDownloadsOnScan)

    [<Test>]
    member public this.``Update feed from RSS file with maximum number of podcasts``() =
        let initialInputPath = workingDirectory + initialRSSWithMaximumPodcastsFileName
        let firstDateTime = new DateTime(2015, 1, 2, 8, 9, 10)
        let secondDateTime = new DateTime(2016, 3, 4, 11, 12, 13)

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(initialRSSWithMaximumPodcastsFileName, initialInputPath)
        let initialFeed = Setup.createTestFeed initialInputPath
        
        let firstPodcast = initialFeed.Podcasts.[0]
        firstPodcast.SetFileDetails firstPodcast.FileDetails.FileSize firstDateTime
        
        let secondPodcast = initialFeed.Podcasts.[1]
        secondPodcast.SetFileDetails secondPodcast.FileDetails.FileSize secondDateTime

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(finalRSSWithMaximumPodcastsFileName, initialInputPath)
        let finalFeed = Setup.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(3, finalFeed.Podcasts.Length)

        Assert.AreEqual(firstPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[1].FileDetails.DownloadDate)
        Assert.AreEqual(secondPodcast.FileDetails.DownloadDate, finalFeed.Podcasts.[2].FileDetails.DownloadDate)

    [<Test>]
    member public this.``Update feed with no podcasts from RSS file with no podcasts``() =
        let initialInputPath = workingDirectory + rssWithNoPodcasts

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let initialFeed = Setup.createTestFeed initialInputPath

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let finalFeed = Setup.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(0, finalFeed.Podcasts.Length)

    [<Test>]
    member public this.``Update feed with no podcasts from RSS file with podcasts``() =
        let initialInputPath = workingDirectory + rssWithNoPodcasts

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithNoPodcasts, initialInputPath)
        let initialFeed = Setup.createTestFeed initialInputPath

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssWithOnePodcast, initialInputPath)
        let finalFeed = Setup.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(1, finalFeed.Podcasts.Length)

    [<Test>]
    member public this.``Update feed with local image paths set``() =
        let initialInputPath = workingDirectory + feedFileName
        let feedImageName = "LocalFeedImagePath"
        let podcastImageName = "LocalPodcastImagePath"

        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(feedFileName, initialInputPath)
        let mutable initialFeed = Setup.createTestFeed initialInputPath
        initialFeed <- Feed.SetImageFileName initialFeed feedImageName
        initialFeed.Podcasts.[0].SetImageFileName podcastImageName
        
        let finalFeed = Setup.updateTestFeed initialFeed

        Assert.AreEqual(initialFeed, finalFeed)
        Assert.AreEqual(feedImageName, finalFeed.ImageFileName)
        Assert.AreEqual(podcastImageName, finalFeed.Podcasts.[0].FileDetails.ImageFileName)

    [<Test>]
    member public this.``Create feed with multiple enclosure tags per item``() =
        let inputPath = workingDirectory + itemWithMultipleEnclosureTags
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(itemWithMultipleEnclosureTags, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(3, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle1of3 podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty

        Asserting.assertPodcastIsCorrect feed.Podcasts.[1] podcastTitle2of3 podcastDescription secondPodcastURL podcastImageURL
            secondPodcastFileSize podcastPubDate String.Empty

        Asserting.assertPodcastIsCorrect feed.Podcasts.[2] podcastTitle3of3 podcastDescription thirdPodcastURL podcastImageURL
            thirdPodcastFileSize podcastPubDate String.Empty

    [<Test>]
    member public this.``Create feed with multiple duplicate enclosure tags per item``() =
        let inputPath = workingDirectory + itemWithMultipleDuplicateEnclosureTags
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(itemWithMultipleDuplicateEnclosureTags, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(1, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty

    [<Test>]
    member public this.``Create feed with multiple mediacontent tags per item``() =
        let inputPath = workingDirectory + itemWithMultipleMediaContentTags
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(itemWithMultipleMediaContentTags, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(3, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle1of3 podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty

        Asserting.assertPodcastIsCorrect feed.Podcasts.[1] podcastTitle2of3 podcastDescription secondPodcastURL podcastImageURL
            secondPodcastFileSize podcastPubDate String.Empty

        Asserting.assertPodcastIsCorrect feed.Podcasts.[2] podcastTitle3of3 podcastDescription thirdPodcastURL podcastImageURL
            thirdPodcastFileSize podcastPubDate String.Empty

    [<Test>]
    member public this.``Create feed with multiple duplicate mediacontent tags per item``() =
        let inputPath = workingDirectory + itemWithMultipleDuplicateMediaContentTags
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(itemWithMultipleDuplicateMediaContentTags, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(1, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty

    [<Test>]
    member public this.``Create feed with enclosure and mediacontent tags per item``() =
        let inputPath = workingDirectory + itemWithEnclosureAndMediaContentTags
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(itemWithEnclosureAndMediaContentTags, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(2, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle1of2 podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty

        Asserting.assertPodcastIsCorrect feed.Podcasts.[1] podcastTitle2of2 podcastDescription secondPodcastURL podcastImageURL
            secondPodcastFileSize podcastPubDate String.Empty

    [<Test>]
    member public this.``Create feed from file with duplicate enclosure and mediacontent tags per item``() =
        let inputPath = workingDirectory + itemWithDuplicateEnclosureAndMediaContentTags
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(itemWithDuplicateEnclosureAndMediaContentTags, inputPath)
        let feed = Setup.createTestFeed inputPath

        Assert.AreNotEqual(null, feed)
        Assert.AreNotEqual(null, feed.Podcasts)
        Assert.AreEqual(1, feed.Podcasts.Length)

        Asserting.assertPodcastIsCorrect feed.Podcasts.[0] podcastTitle podcastDescription firstPodcastURL podcastImageURL
            firstPodcastFileSize podcastPubDate String.Empty

    [<Test>]
    member public this.``Download file is backed up when updating feed``() =

        let inputPath = workingDirectory + rssFileName
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)

        let feedFilePath = workingDirectory + "download.rss"
        let feed = Setup.createTestFeedUsingDownloadFile inputPath feedFilePath

        let downloadBackupExistsBeforeSecondFeedCreation = System.IO.File.Exists(feedFilePath + ".bak")

        Assert.AreEqual(false, downloadBackupExistsBeforeSecondFeedCreation)

        Setup.updateTestFeedUsingDownloadFile feed feedFilePath |> ignore

        let downloadBackupExistsAfterSecondFeedCreation = System.IO.File.Exists(feedFilePath + ".bak")

        Assert.AreEqual(true, downloadBackupExistsAfterSecondFeedCreation)

    [<Test>]
    member public this.``Backup download file is replaced when updating feed``() =

        let inputPath = workingDirectory + rssFileName
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile(rssFileName, inputPath)

        let feedFilePath = workingDirectory + "download.rss"
        let feed = Setup.createTestFeedUsingDownloadFile inputPath feedFilePath
        let feed = Setup.updateTestFeedUsingDownloadFile feed feedFilePath

        let firstFileWriteTime = System.IO.File.GetLastWriteTime(feedFilePath + ".bak");

        Setup.updateTestFeedUsingDownloadFile feed feedFilePath |> ignore
        let secondFileWriteTime = System.IO.File.GetLastWriteTime(feedFilePath + ".bak")

        let firstWriteTimeIsBeforeSecondWriteTime = firstFileWriteTime < secondFileWriteTime
        Assert.AreEqual(true, firstWriteTimeIsBeforeSecondWriteTime)

     