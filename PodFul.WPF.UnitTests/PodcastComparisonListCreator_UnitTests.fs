namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.Library
open PodFul.TestSupport
open PodFul.WPF.Processing

type PodcastComparisonListCreator_UnitTests() =

    member private this.AssertPodcastComparisonIsCorrect expectedOldTitle expectedNewTitle (podcastComparison : PodcastComparison) = 
        Assert.AreEqual(expectedOldTitle, podcastComparison.OldTitle)
        Assert.AreEqual(expectedNewTitle, podcastComparison.NewTitle)

    member private this.CreatePodcast title = 
        Setup.createTestPodcast title "Description" "url" DateTime.MinValue 0L DateTime.MinValue "imagefilename" "imageurl" String.Empty

    member private this.CreatePodcastList titles =
        [for title in titles do
            yield this.CreatePodcast title
        ] |> List.toArray

    [<Test>]
    member public this.``New Podcast list has two new podcasts and lost one.``() =
        // new podcast list has two new podcasts and lost an old one
        let oldPodcasts = this.CreatePodcastList [| "C"; "D"; "E" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "C"; "D"|]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(5, list.Count)
        Assert.AreEqual(PodcastComparison.NoMatch, list.[0].OldTitle)
        Assert.AreEqual("1. A", list.[0].NewTitle)

        Assert.AreEqual(PodcastComparison.NoMatch, list.[1].OldTitle)
        Assert.AreEqual("2. B", list.[1].NewTitle)

        Assert.AreEqual("1. C", list.[2].OldTitle)
        Assert.AreEqual("3. C", list.[2].NewTitle)

        Assert.AreEqual("2. D", list.[3].OldTitle)
        Assert.AreEqual("4. D", list.[3].NewTitle)

        Assert.AreEqual("3. E", list.[4].OldTitle)
        Assert.AreEqual(PodcastComparison.NoMatch, list.[4].NewTitle)

    [<Test>]
    member public this.``New Podcast list has one more podcasts only.``() =
        let oldPodcasts = this.CreatePodcastList [| "B"; "C" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "C" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(3, list.Count)
        Assert.AreEqual(PodcastComparison.NoMatch, list.[0].OldTitle)
        Assert.AreEqual("1. A", list.[0].NewTitle)
        Assert.AreEqual("1. B", list.[1].OldTitle)
        Assert.AreEqual("2. B", list.[1].NewTitle)
        Assert.AreEqual("2. C", list.[2].OldTitle)
        Assert.AreEqual("3. C", list.[2].NewTitle)

    [<Test>]
    member public this.``First Podcast from both lists are not equal but all other podcasts are equal.``() =
        let oldPodcasts = this.CreatePodcastList [| "B"; "C"; "D" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "C"; "D" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(4, list.Count)
        this.AssertPodcastComparisonIsCorrect PodcastComparison.NoMatch "1. A" list.[0]
        this.AssertPodcastComparisonIsCorrect "1. B" PodcastComparison.NoMatch list.[1]
        this.AssertPodcastComparisonIsCorrect "2. C" "2. C" list.[2]
        this.AssertPodcastComparisonIsCorrect "3. D" "3. D" list.[3]
