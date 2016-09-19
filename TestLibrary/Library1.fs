namespace TestLibrary

open System.IO
open System.Reflection
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library

type Test_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\ImageResolver_IntergrationTests\";

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Local path not set and URL path is set so file is downloaded and file path is returned``() =
        
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"
        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = ""
        let expectedPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"
        let imageResolver = ImageResolver(imageDirectory, defaultImagePath, false, renameFunction) :> IImageResolver

        //let assembly = Assembly.GetExecutingAssembly()
        //assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        // Act
        let resultPath = imageResolver.GetName2 localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, expectedPath)
        //resultPath |> should equal expectedPath
        //File.Exists(expectedPath) |> should equal true
