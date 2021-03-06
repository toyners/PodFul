﻿namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open PodFul.WPF.Processing

type PodcastComparisonListCreator_UnitTests() =

    member private this.AssertPodcastComparisonIsCorrect expectedOldTitle expectedNewTitle expectedIndex (podcastComparison : PodcastComparison) = 
        Assert.AreEqual(expectedOldTitle, podcastComparison.OldTitle)
        Assert.AreEqual(expectedNewTitle, podcastComparison.NewTitle)
        Assert.AreEqual(expectedIndex, podcastComparison.Index)

    member private this.CreatePodcast title = 
        Setup.createTestPodcast title "Description" "url" DateTime.MinValue 0L DateTime.MinValue "imagefilename" "imageurl" String.Empty

    member private this.CreatePodcastList titles =
        [for title in titles do
            yield this.CreatePodcast title
        ] |> List.toArray

    [<Test>]
    member public this.``New Podcast list has two new podcasts and lost one.``() =
        let oldPodcasts = this.CreatePodcastList [| "C"; "D"; "E" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "C"; "D"|]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(5, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. A" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "2. B" 1 list.[1]
        this.AssertPodcastComparisonIsCorrect "1. C" "3. C" 2 list.[2]
        this.AssertPodcastComparisonIsCorrect "2. D" "4. D" 3 list.[3]
        this.AssertPodcastComparisonIsCorrect "3. E" PodcastComparison.NoMatch -1 list.[4]

    [<Test>]
    member public this.``New Podcast list has one more podcasts only.``() =
        let oldPodcasts = this.CreatePodcastList [| "B"; "C" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "C" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(3, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. A" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect "1. B" "2. B" 1 list.[1]
        this.AssertPodcastComparisonIsCorrect "2. C" "3. C" 2 list.[2]

    [<Test>]
    member public this.``First Podcast from both lists are not equal but all other podcasts are equal.``() =
        let oldPodcasts = this.CreatePodcastList [| "B"; "C"; "D" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "C"; "D" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(4, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. A" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect "1. B" PodcastComparison.NoMatch -1 list.[1]
        this.AssertPodcastComparisonIsCorrect "2. C" "2. C" 1 list.[2]
        this.AssertPodcastComparisonIsCorrect "3. D" "3. D" 2 list.[3]

    [<Test>]
    member public this.``Middle Podcast from both lists are not equal but all other podcasts are equal.``() =
        let oldPodcasts = this.CreatePodcastList [| "A"; "B"; "D" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "C"; "D" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(4, list.Count)
        this.AssertPodcastComparisonIsCorrect "1. A" "1. A" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "2. C" 1 list.[1]
        this.AssertPodcastComparisonIsCorrect "2. B" PodcastComparison.NoMatch -1 list.[2]
        this.AssertPodcastComparisonIsCorrect "3. D" "3. D" 2 list.[3]

    [<Test>]
    member public this.``Last Podcast from both lists are not equal but all other podcasts are equal.``() =
        let oldPodcasts = this.CreatePodcastList [| "A"; "B"; "C" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "D" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(4, list.Count)
        this.AssertPodcastComparisonIsCorrect "1. A" "1. A" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect "2. B" "2. B" 1 list.[1]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "3. D" 2 list.[2]
        this.AssertPodcastComparisonIsCorrect "3. C" PodcastComparison.NoMatch -1 list.[3]

    [<Test>]
    member public this.``New list has lost one and gained one but all other podcasts are equal.``() =
        let oldPodcasts = this.CreatePodcastList [| "B"; "C"; "D" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "C" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(4, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. A" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect "1. B" "2. B" 1 list.[1]
        this.AssertPodcastComparisonIsCorrect "2. C" "3. C" 2 list.[2]
        this.AssertPodcastComparisonIsCorrect "3. D" PodcastComparison.NoMatch -1 list.[3]

    [<Test>]
    member public this.``No matches in lists of same size.``() =
        let oldPodcasts = this.CreatePodcastList [| "A"; "B"; "C" |]
        let newPodcasts = this.CreatePodcastList [| "D"; "E"; "F" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(6, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. D" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect "1. A" PodcastComparison.NoMatch -1 list.[1]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "2. E" 1 list.[2]
        this.AssertPodcastComparisonIsCorrect "2. B" PodcastComparison.NoMatch -1 list.[3]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "3. F" 2 list.[4]
        this.AssertPodcastComparisonIsCorrect "3. C" PodcastComparison.NoMatch -1 list.[5]

    [<Test>]
    member public this.``No matches when new list is larger.``() =
        let oldPodcasts = this.CreatePodcastList [| "A"; "B"; |]
        let newPodcasts = this.CreatePodcastList [| "D"; "E"; "F" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(5, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. D" 0 list.[0]
        this.AssertPodcastComparisonIsCorrect "1. A" PodcastComparison.NoMatch -1 list.[1]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "2. E" 1 list.[2]
        this.AssertPodcastComparisonIsCorrect "2. B" PodcastComparison.NoMatch -1 list.[3]
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "3. F" 2 list.[4]
