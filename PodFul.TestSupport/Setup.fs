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

    let createTestFeedFromParameters title description website directory url imageURL imageFileName creationDateTime updatedDateTime doScan completeDownloadsOnScan deliverDownloadsOnScan = 
      {
        Title = title
        Description = description
        Website = website
        Directory = directory
        URL = url
        ImageURL = imageURL
        ImageFileName = imageFileName
        Podcasts = [||]
        CreationDateTime = creationDateTime
        UpdatedDateTime = updatedDateTime
        DoScan = doScan
        CompleteDownloadsOnScan = completeDownloadsOnScan
        DeliverDownloadsOnScan = deliverDownloadsOnScan
      }

    let createTestPodcast title description url pubDate fileSize downloadDate imageFileName imageURL fileName =
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
          FileName = fileName
        }
      }

    let updateTestFeed feed =
      FeedFunctions.UpdateFeed feed null CancellationToken.None
