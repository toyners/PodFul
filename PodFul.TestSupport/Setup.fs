﻿namespace PodFul.TestSupport

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
            Podcasts = null
            CreationDateTime = new DateTime(2016, 2, 3)
            UpdatedDateTime = FeedFunctions.NoDateTime
            DoScan = true
            CompleteDownloadsOnScan = true
            DeliverDownloadsOnScan = true
        }

    let createTestFeed url =
        FeedFunctions.CreateFeed url null "DirectoryPath" null CancellationToken.None

    let createTestFeedWithDirectoryPath url directoryPath =
        FeedFunctions.CreateFeed url null directoryPath null CancellationToken.None

    let createTestFeedUsingDownloadFile url downloadFilePath = 
        FeedFunctions.CreateFeed url downloadFilePath "DirectoryPath" null CancellationToken.None

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null null CancellationToken.None