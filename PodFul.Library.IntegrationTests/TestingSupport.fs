namespace PodFul.Library.IntegrationTests

open PodFul.Library
open System.Threading

module public TestingSupport = 

    let createTestFeed url directoryPath =
        FeedFunctions.CreateFeed url directoryPath null CancellationToken.None

    let updateTestFeed feed =
        FeedFunctions.UpdateFeed feed null CancellationToken.None


