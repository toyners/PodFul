namespace PodFul.TestSupport

open PodFul.Library
open System
open System.Threading

module public Setup = 

    let createDefaultTestFeed =
        {
            Title = "title"
            Description = "description"
            Website = "website"
            Directory = "directory"
            URL = "url"
            ImageURL = "image URL"
            ImageFileName = ""
            Podcasts = [||]
            CreationDateTime = new DateTime(2016, 2, 3)
            UpdatedDateTime = FeedFunctions.NoDateTime
            DoScan = true
            CompleteDownloadsOnScan = true
            DeliverDownloadsOnScan = true
        }

    let createTestFeed url =
        FeedFunctions.CreateFeed url null "DirectoryPath" "" CancellationToken.None

    let createTestFeedWithDirectoryPath url directoryPath =
        FeedFunctions.CreateFeed url null directoryPath "" CancellationToken.None

    let createTestFeedUsingDownloadFile url downloadFilePath = 
        FeedFunctions.CreateFeed url downloadFilePath "DirectoryPath" "" CancellationToken.None

    let createTestPodcast title description url pubDate fileSize downloadDate imageFileName imageURL =
        {
            Title = title
            Description = description
            URL = url
            ImageURL = imageURL
            PubDate = pubDate
            FileDetails =
                {
                    FileSize = fileSize
                    DownloadDate = downloadDate
                    ImageFileName = imageFileName
                }
        }

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null CancellationToken.None
