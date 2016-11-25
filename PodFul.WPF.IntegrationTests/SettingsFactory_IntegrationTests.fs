namespace PodFul.WPF.IntegrationTests

open System
//open FsUnit
open NUnit.Framework
open PodFul.WPF

type SettingsFactory_IntegrationTests() =

    [<Test>]
    member public this.``Filepath is null so default settings object is returned``() =
        let settings = SettingsFactory.Create(null, null)

        Assert.AreNotSame(settings, null)
        Assert.AreEqual(settings.ConcurrentDownloadCount, 3)
        Assert.AreEqual(settings.ConfirmPodcastDownloadThreshold, 3)

    [<Test>]
    member public this.``Filepath is empty so default settings object is returned``() =
        let settings = SettingsFactory.Create(null, null)

        Assert.AreNotSame(settings, null)
        Assert.AreEqual(settings.ConcurrentDownloadCount, 3)
        Assert.AreEqual(settings.ConfirmPodcastDownloadThreshold, 3)

    [<Test>]
    member public this.``Filepath does not exist so default settings object is returned``() =
        let settings = SettingsFactory.Create(null, null)

        Assert.AreNotSame(settings, null)
        Assert.AreEqual(settings.ConcurrentDownloadCount, 3)
        Assert.AreEqual(settings.ConfirmPodcastDownloadThreshold, 3)
