﻿namespace PodFul.WPF.IntegrationTests

open System
open System.Windows.Controls;
open System.Windows.Controls.Primitives;
open System.IO
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library
open PodFul.WPF.Logging
open PodFul.WPF.Miscellaneous
open PodFul.FileDelivery

type Settings_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.WPF.IntegrationTests\Test\Settings_IntegrationTests\";

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``File path is null and causes a meaningful exception to be thrown``() =

        let mutable testSuccessful = false
        try
            new Settings(null) |> ignore
        with
        | _ as e ->
            let isNullReferenceException = e :? System.NullReferenceException
            Assert.IsTrue(isNullReferenceException)

            let nullReferenceException = e :?> System.NullReferenceException
            Assert.AreEqual(nullReferenceException.Message, "Parameter 'filePath' is null.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    member public this.``File path is empty and causes a meaningful exception to be thrown``() =

        let mutable testSuccessful = false
        try
            new Settings(String.Empty) |> ignore
        with
        | _ as e ->
            let isArgumentException = e :? System.ArgumentException
            Assert.IsTrue(isArgumentException)

            let argumentReferenceException = e :?> System.ArgumentException
            Assert.AreEqual(argumentReferenceException.Message, "Parameter 'filePath' is empty.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    [<TestCase(@"C:\BadCharacter?InFolder\FileName.txt")>]
    [<TestCase(@"C:\BadCharacter?InFileName.txt")>]
    member public this.``Filepath is illegal and causes a meaningful exception to be thrown``(badFilePath : string) =

        let mutable testSuccessful = false
        try
            new Settings(badFilePath) |> ignore
        with
        | _ as e ->
            let isArgumentException = e :? System.ArgumentException
            Assert.IsTrue(isArgumentException)

            let argumentReferenceException = e :?> System.ArgumentException
            Assert.AreEqual(argumentReferenceException.Message, "Parameter 'filePath' contains characters that are illegal in file paths.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    [<TestCase(@"..\Folder\FileName.txt")>]
    [<TestCase(@".\FileName.txt")>]
    [<TestCase(@"FileName.txt")>]
    member public this.``Filepath is relative and causes a meaningful exception to be thrown``(relativeFilePath : string) =

        let mutable testSuccessful = false
        try
            new Settings(relativeFilePath) |> ignore
        with
        | _ as e ->
            let isArgumentException = e :? System.ArgumentException
            Assert.IsTrue(isArgumentException)

            let argumentReferenceException = e :?> System.ArgumentException
            Assert.AreEqual(argumentReferenceException.Message, "Parameter 'filePath' contains a relative path.")

            testSuccessful <- true

        Assert.IsTrue(testSuccessful)

    [<Test>]
    member public this.``File does not exist so default properties are set on settings object``() =
        let nonExistantFilePath = workingDirectory + @"FileDoesNotExist.settings"
        let settings = new Settings(nonExistantFilePath)

        Assert.IsNotNull(settings)
        Assert.AreEqual(settings.ConcurrentDownloadCount, 3)

    [<Test>]
    member public this.``Saving settings results in file being created``() =
        
        // Arrange
        let filePath = workingDirectory + @"Settings.settings"
        let fileExistsBeforeSave = File.Exists(filePath)
        let settings = new Settings(filePath)
        
        // Act
        settings.Save()

        // Assert
        Assert.IsFalse(fileExistsBeforeSave)
        Assert.IsTrue(File.Exists(filePath))

    [<Test>]
    member public this.``Created settings object from file has same values as original settings object``() =
        
        // Arrange
        let filePath = workingDirectory + @"Settings.settings"
        let settingsA = new Settings(filePath)

        settingsA.Save()

        // Act
        let settingsB = new Settings(filePath)

        // Assert
        Assert.AreNotSame(settingsA, settingsB)
        Assert.AreEqual(settingsA.ConcurrentDownloadCount, settingsB.ConcurrentDownloadCount)
        Assert.AreEqual(settingsA.DeliveryPointData.Count, settingsB.DeliveryPointData.Count)
        