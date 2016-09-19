namespace PodFul.Library.IntegrationTests

open System
open System.IO
open System.Reflection
open FsUnit
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library

type ImageResolver_IntegrationTests() =

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

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        // Act
        let resultPath = imageResolver.GetName2 localPath urlPath
        
        // Assert
        resultPath |> should equal expectedPath
        File.Exists(expectedPath) |> should equal true
    
    (*[<Test>]
    member public this.``Local path set and file exists so local path is returned``()=
        ignore

    [<Test>]
    member public this.``Local path not set and URL path not set so default path is returned``()=
        ignore

    [<Test>]
    member public this.``Local path set but file not found locally so is downloaded and local path is returned``()=
        ignore

    [<Test>]
    member public this.``Local name set to default but file not found locally so default name is returned``()=
        ignore

    [<Test>]
    member public this.``Local name set to default and URL name is set so file is downloaded and updated local name is returned``()=
        ignore
        *)