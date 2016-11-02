namespace PodFul.WPF.UnitTests

open System
open NUnit.Framework
open PodFul.Library
open PodFul.WPF.Processing

type PodcastComparisonListCreator_UnitTests() =

    member private this.CreatePodcast title = 
        {
            Title = title
            Description = "Description"
            URL = "url"
            ImageURL = "imageurl"
            PubDate = DateTime.MinValue
            FileDetails = 
            {
                FileSize = 0L
                DownloadDate = DateTime.MinValue
                ImageFileName = "imagefilename"
            }
        }

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
        Assert.AreEqual("(no match)", list.[0].OldTitle)
        Assert.AreEqual("A", list.[0].NewTitle)
        Assert.AreEqual("(no match)", list.[1].OldTitle)
        Assert.AreEqual("B", list.[1].NewTitle)
        Assert.AreEqual("C", list.[2].OldTitle)
        Assert.AreEqual("C", list.[2].NewTitle)
        Assert.AreEqual("D", list.[3].OldTitle)
        Assert.AreEqual("D", list.[3].NewTitle)
        Assert.AreEqual("E", list.[4].OldTitle)
        Assert.AreEqual("(no match)", list.[4].NewTitle)

    [<Test>]
    member public this.``New Podcast list has one more podcasts only.``() =
        let oldPodcasts = this.CreatePodcastList [| "B"; "C" |]
        let newPodcasts = this.CreatePodcastList [| "A"; "B"; "C" |]

        // Act
        let list = PodcastComparisonListCreator.Create(oldPodcasts, newPodcasts)
        
        // Assert
        Assert.AreEqual(3, list.Count)
        Assert.AreEqual("(no match)", list.[0].OldTitle)
        Assert.AreEqual("A", list.[0].NewTitle)
        Assert.AreEqual("B", list.[1].OldTitle)
        Assert.AreEqual("B", list.[1].NewTitle)
        Assert.AreEqual("C", list.[2].OldTitle)
        Assert.AreEqual("C", list.[2].NewTitle)
