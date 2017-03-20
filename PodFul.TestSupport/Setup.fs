namespace PodFul.TestSupport

open PodFul.Library
open System.Threading

module public Setup = 

    let createTestFeed url directoryPath =
        FeedFunctions.CreateFeed url null directoryPath null CancellationToken.None

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null null CancellationToken.None
