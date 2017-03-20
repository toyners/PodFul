namespace PodFul.TestSupport

open PodFul.Library
open System.Threading

module public Setup = 

    let createTestFeed url =
        FeedFunctions.CreateFeed url null "DirectoryPath" null CancellationToken.None

    let createTestFeedWithDirectoryPath url directoryPath =
        FeedFunctions.CreateFeed url null directoryPath null CancellationToken.None

    let createTestFeedUsingDownloadFile url downloadFilePath = 
        FeedFunctions.CreateFeed url downloadFilePath "DirectoryPath" null CancellationToken.None

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null null CancellationToken.None
