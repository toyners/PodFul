namespace PodFul.WPF.UnitTests

open System
open System.IO
open Jabberwocky.Toolkit.IO
open PodFul.Library
open PodFul.TestSupport
open PodFul.WPF.Miscellaneous
open NUnit.Framework

type PodcastProperties_IntegrationTests() = 

    let workingDirectory = @"C:\Projects\PodFul\PodFul.WPF.IntegrationTests\Test\PodcastProperties_IntegrationTests\";

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Podcast properties instance created from podcast with existing file returns file path``() =
        let url = "podcast.mp3"
        let filePath = workingDirectory + url
        File.Create(filePath).Dispose();
        let podcast = Setup.createTestPodcast "title" "description" url DateTime.Now 1L DateTime.Now "image file name" "image url" url
        let podcastProperties = new PodcastProperties(podcast, workingDirectory)
        
        Assert.AreEqual(filePath, podcastProperties.FilePath)

    [<Test>]
    member public this.``Podcast properties instance created from podcast with no existing file returns No File in file path``() =
        let url = "podcast.mp3"
        let podcast = Setup.createTestPodcast "title" "description" url DateTime.Now 1L DateTime.Now "image file name" "image url" String.Empty
        let podcastProperties = new PodcastProperties(podcast, workingDirectory)
        
        Assert.AreEqual("<No File>", podcastProperties.FilePath)

    [<Test>]
    member public this.``Podcast properties instance created from podcast with no download date returns No Download in file path``() =
        let podcast = Setup.createTestPodcast "title" "description" "url" DateTime.Now 1L FeedFunctions.NoDateTime "image file name" "image url" String.Empty
        let podcastProperties = new PodcastProperties(podcast, "")
        
        Assert.AreEqual("<No Download>", podcastProperties.FilePath)
