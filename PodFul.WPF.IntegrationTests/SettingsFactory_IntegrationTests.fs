namespace PodFul.WPF.IntegrationTests

open System
open System.IO
open NUnit.Framework
open PodFul.WPF
open PodFul.WPF.Logging

type SettingsFactory_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\SettingsFactory_IntegrationTests\";

    [<Test>]
    member public this.``File path is null and causes a meaningful exception to be thrown``() =

        let mutable testSuccessful = false
        try
            new Settings(null, null) |> ignore
        with
        | _ as e ->
            let isNullReferenceException = e :? System.NullReferenceException
            Assert.IsTrue(isNullReferenceException)

            let nullReferenceException = e :?> System.NullReferenceException
            Assert.AreEqual(nullReferenceException.Message, "Parameter 'fileDeliveryLogger' is null.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    member public this.``File path is empty and causes a meaningful exception to be thrown``() =

        let mutable testSuccessful = false
        try
            new Settings(String.Empty, null) |> ignore
        with
        | _ as e ->
            let isArgumentException = e :? System.ArgumentException
            Assert.IsTrue(isArgumentException)

            let argumentReferenceException = e :?> System.ArgumentException
            Assert.AreEqual(argumentReferenceException.Message, "Parameter 'filePath' is empty.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    member public this.``Filepath is illegal and causes a meaningful exception to be thrown``() =

        let mutable testSuccessful = false
        try
            new Settings(@"C:\BadCharacter?InName", null) |> ignore
        with
        | _ as e ->
            let isArgumentException = e :? System.ArgumentException
            Assert.IsTrue(isArgumentException)

            let argumentReferenceException = e :?> System.ArgumentException
            Assert.AreEqual(argumentReferenceException.Message, "Parameter 'filePath' contains characters that are illegal in file paths.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    member public this.``File delivery logger is null and causes a meaningful exception to be thrown``() =

        let mutable testSuccessful = false
        try
            new Settings(@"C:\test.txt", null) |> ignore
        with
        | _ as e ->
            let isNullReferenceException = e :? System.NullReferenceException
            Assert.IsTrue(isNullReferenceException)

            let nullReferenceException = e :?> System.NullReferenceException
            Assert.AreEqual(nullReferenceException.Message, "Parameter 'fileDeliveryLogger' is null.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    member public this.``File does not exist so default properties are set on settings object``() =
        let nonExistantFilePath = @"C:\File\Does\Not\Exist.xml"
        let fileDeliveryLogger = new FileDeliveryLogger()
        let settings = new Settings(nonExistantFilePath, fileDeliveryLogger)

        Assert.IsNotNull(settings)
        Assert.AreEqual(settings.ConcurrentDownloadCount, 3)
        Assert.AreEqual(settings.ConfirmPodcastDownloadThreshold, 3)

    [<Test>]
    member public this.``Saving settings results in file being created``() =
        let fileDeliveryLogger = new FileDeliveryLogger()
        let filePath = workingDirectory + @"file.xml"
        let settings = new Settings(filePath, fileDeliveryLogger)

        Assert.IsFalse(File.Exists(filePath))

        settings.Save()

        Assert.IsTrue(File.Exists(filePath))

    [<Test>]
    member public this.``Full cycle``() =
        let fileDeliveryLogger = new FileDeliveryLogger()
        let filePath = workingDirectory + @"file"
        let settingsA = SettingsFactory.Create(filePath, fileDeliveryLogger)

        SettingsFactory.Save(filePath, settingsA)

        let settingsB = SettingsFactory.Create(filePath, fileDeliveryLogger)

        Assert.AreNotSame(settingsA, settingsB)
        Assert.AreEqual(settingsA.ConcurrentDownloadCount, settingsB.ConcurrentDownloadCount)
        Assert.AreEqual(settingsA.ConfirmPodcastDownloadThreshold, settingsB.ConfirmPodcastDownloadThreshold)
        //Assert.(settingsA.ConfirmPodcastDownloadThreshold, settingsB.ConfirmPodcastDownloadThreshold)
        