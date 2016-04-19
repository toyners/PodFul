namespace PodFul.Library.IntegrationTests

open FsUnit
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open System
open System.IO
open System.Reflection

type FeedFunctions_IntergrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\FeedFunctions_IntergrationTests\";

    let feedTitle = "Feed Title"
    let feedDescription = "Feed Description"
    let feedWebsite = "Feed Website"
    let feedDirectory = "Feed Directory"
    let feedFeed = "Feed Feed"

    let feedDescriptionContainingLineBreaks = "This\r\nis\r\n\r\na\nFeed\n\nDescription."
    let expectedFeedDescription = "This is a Feed Description."

    let firstPodcastTitle = "Podcast #1 Title"
    let firstPodcastDescription = "Podcast #1 Description"
    let firstPodcastURL = "Podcast1.mp3"
    let firstPodcastFileSize = 1L
    let firstPodcastPubDate = new DateTime(2014, 1, 2, 1, 2, 3)

    let podcastDescriptionContainingLineBreaks = "This\r\nis\r\n\r\na\nPodcast\n\nDescription."
    let expectedPodcastDescription = "This is a Podcast Description."

    let secondPodcastTitle = "Podcast #2 Title"
    let secondPodcastDescription = "Podcast #2 Description"
    let secondPodcastURL = "Podcast2.mp3"
    let secondPodcastFileSize = 2L
    let secondPodcastPubDate = new DateTime(2015, 3, 4, 10, 11, 12)

    let thirdPodcastTitle = "Podcast #3 Title"
    let thirdPodcastDescription = "Podcast #3 Description"
    let thirdPodcastURL = "Podcast3.mp3"
    let thirdPodcastFileSize = 3L
    let thirdPodcastPubDate = new DateTime(2016, 5, 6, 15, 16, 17)

    let firstDownloadDate = new DateTime(2017, 1, 1)
    let latestDownloadDate = new DateTime(2017, 1, 2)

    member private this.CreateFeed =
        {
            Title = feedTitle
            Description = feedDescription
            Website = feedWebsite
            Directory = feedDirectory
            URL = feedFeed
            Podcasts = 
            [|
                {
                    Title = firstPodcastTitle
                    Description = firstPodcastDescription
                    URL = firstPodcastURL
                    FileSize = firstPodcastFileSize
                    PubDate = firstPodcastPubDate
                    FirstDownloadDate = firstDownloadDate
                    LatestDownloadDate = latestDownloadDate
                };
                {
                    Title = secondPodcastTitle
                    Description = secondPodcastDescription
                    URL = secondPodcastURL
                    FileSize = secondPodcastFileSize
                    PubDate = secondPodcastPubDate
                    FirstDownloadDate = firstDownloadDate
                    LatestDownloadDate = latestDownloadDate
                };
                {
                    Title = thirdPodcastTitle
                    Description = thirdPodcastDescription
                    URL = thirdPodcastURL
                    FileSize = thirdPodcastFileSize
                    PubDate = thirdPodcastPubDate
                    FirstDownloadDate = firstDownloadDate
                    LatestDownloadDate = latestDownloadDate
                };            
            |]
        }

    member private this.CreateFeedWithTextContainingLineBreaks =
        {
            Title = feedTitle
            Description = feedDescriptionContainingLineBreaks
            Website = feedWebsite
            Directory = feedDirectory
            URL = feedFeed
            Podcasts = 
            [|
                {
                    Title = firstPodcastTitle
                    Description = podcastDescriptionContainingLineBreaks
                    URL = firstPodcastURL
                    FileSize = firstPodcastFileSize
                    PubDate = firstPodcastPubDate
                    FirstDownloadDate = DateTime.MinValue
                    LatestDownloadDate = DateTime.MinValue
                }
            |]
        }

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Create Feed from RSS url``() =
        let inputPath = workingDirectory + "podcast.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath)
        let feed = FeedFunctions.CreateFeed inputPath "DirectoryPath"

        feed |> should not' (equal null)
        feed.Title |> should equal feedTitle
        feed.Description |> should equal feedDescription
        feed.Website |> should equal feedWebsite
        feed.Directory |> should equal "DirectoryPath"
        feed.URL |> should equal inputPath

        feed.Podcasts |> should not' (be null)
        feed.Podcasts.Length |> should equal 3

        feed.Podcasts.[0].Title |> should equal firstPodcastTitle
        feed.Podcasts.[0].Description |> should equal firstPodcastDescription
        feed.Podcasts.[0].URL |> should equal firstPodcastURL
        feed.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        feed.Podcasts.[0].PubDate |> should equal firstPodcastPubDate

        feed.Podcasts.[1].Title |> should equal secondPodcastTitle
        feed.Podcasts.[1].Description |> should equal secondPodcastDescription
        feed.Podcasts.[1].URL |> should equal secondPodcastURL
        feed.Podcasts.[1].FileSize |> should equal secondPodcastFileSize
        feed.Podcasts.[1].PubDate |> should equal secondPodcastPubDate

        feed.Podcasts.[2].Title |> should equal thirdPodcastTitle
        feed.Podcasts.[2].Description |> should equal thirdPodcastDescription
        feed.Podcasts.[2].URL |> should equal thirdPodcastURL
        feed.Podcasts.[2].FileSize |> should equal thirdPodcastFileSize
        feed.Podcasts.[2].PubDate |> should equal thirdPodcastPubDate

    [<Test>]
    member public this.``Writing/Reading cycle of Feed record``() = 
        let testRecord = this.CreateFeed
        let outputPath = workingDirectory + "record.txt";
        FeedFunctions.WriteFeedToFile testRecord outputPath

        File.Exists(outputPath) |> should be True

        let inputPath = outputPath
        let resultRecord = FeedFunctions.ReadFeedFromFile(inputPath)

        resultRecord |> should not' (be sameAs testRecord)

        resultRecord.Title |> should equal feedTitle
        resultRecord.Description |> should equal feedDescription
        resultRecord.Website |> should equal feedWebsite
        resultRecord.Directory |> should equal feedDirectory
        resultRecord.URL |> should equal feedFeed

        resultRecord.Podcasts |> should not' (be null)
        resultRecord.Podcasts.Length |> should equal 3

        resultRecord.Podcasts.[0].Title |> should equal firstPodcastTitle
        resultRecord.Podcasts.[0].Description |> should equal firstPodcastDescription
        resultRecord.Podcasts.[0].URL |> should equal firstPodcastURL
        resultRecord.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        resultRecord.Podcasts.[0].PubDate |> should equal firstPodcastPubDate
        resultRecord.Podcasts.[0].FirstDownloadDate |> should equal firstDownloadDate
        resultRecord.Podcasts.[0].LatestDownloadDate |> should equal latestDownloadDate

        resultRecord.Podcasts.[1].Title |> should equal secondPodcastTitle
        resultRecord.Podcasts.[1].Description |> should equal secondPodcastDescription
        resultRecord.Podcasts.[1].URL |> should equal secondPodcastURL
        resultRecord.Podcasts.[1].FileSize |> should equal secondPodcastFileSize
        resultRecord.Podcasts.[1].PubDate |> should equal secondPodcastPubDate
        resultRecord.Podcasts.[1].FirstDownloadDate |> should equal firstDownloadDate
        resultRecord.Podcasts.[1].LatestDownloadDate |> should equal latestDownloadDate

        resultRecord.Podcasts.[2].Title |> should equal thirdPodcastTitle
        resultRecord.Podcasts.[2].Description |> should equal thirdPodcastDescription
        resultRecord.Podcasts.[2].URL |> should equal thirdPodcastURL
        resultRecord.Podcasts.[2].FileSize |> should equal thirdPodcastFileSize
        resultRecord.Podcasts.[2].PubDate |> should equal thirdPodcastPubDate
        resultRecord.Podcasts.[2].FirstDownloadDate |> should equal firstDownloadDate
        resultRecord.Podcasts.[2].LatestDownloadDate |> should equal latestDownloadDate

    [<Test>]
    member public this.``Writing/Reading cycle of Feed record with line breaks in Descriptions``() = 
        let testRecord = this.CreateFeedWithTextContainingLineBreaks 
        let outputPath = workingDirectory + "record.txt";
        FeedFunctions.WriteFeedToFile testRecord outputPath

        File.Exists(outputPath) |> should be True

        let inputPath = outputPath
        let resultRecord = FeedFunctions.ReadFeedFromFile(inputPath)

        resultRecord |> should not' (be sameAs testRecord)

        resultRecord.Title |> should equal feedTitle
        resultRecord.Description |> should equal expectedFeedDescription
        resultRecord.Website |> should equal feedWebsite
        resultRecord.Directory |> should equal feedDirectory
        resultRecord.URL |> should equal feedFeed

        resultRecord.Podcasts |> should not' (be null)
        resultRecord.Podcasts.Length |> should equal 1

        resultRecord.Podcasts.[0].Title |> should equal firstPodcastTitle
        resultRecord.Podcasts.[0].Description |> should equal expectedPodcastDescription
        resultRecord.Podcasts.[0].URL |> should equal firstPodcastURL
        resultRecord.Podcasts.[0].FileSize |> should equal firstPodcastFileSize
        resultRecord.Podcasts.[0].PubDate |> should equal firstPodcastPubDate

    [<Test>]
    member public this.``Create podcast list from RSS url``() =
        let inputPath = workingDirectory + "podcast.rss";
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("podcast.rss", inputPath)
        let podcasts = FeedFunctions.CreatePodcastList inputPath

        podcasts |> should not' (be null)
        podcasts.Length |> should equal 3

        podcasts.[0].Title |> should equal firstPodcastTitle
        podcasts.[0].Description |> should equal firstPodcastDescription
        podcasts.[0].URL |> should equal firstPodcastURL
        podcasts.[0].FileSize |> should equal firstPodcastFileSize
        podcasts.[0].PubDate |> should equal firstPodcastPubDate

        podcasts.[1].Title |> should equal secondPodcastTitle
        podcasts.[1].Description |> should equal secondPodcastDescription
        podcasts.[1].URL |> should equal secondPodcastURL
        podcasts.[1].FileSize |> should equal secondPodcastFileSize
        podcasts.[1].PubDate |> should equal secondPodcastPubDate

        podcasts.[2].Title |> should equal thirdPodcastTitle
        podcasts.[2].Description |> should equal thirdPodcastDescription
        podcasts.[2].URL |> should equal thirdPodcastURL
        podcasts.[2].FileSize |> should equal thirdPodcastFileSize
        podcasts.[2].PubDate |> should equal thirdPodcastPubDate