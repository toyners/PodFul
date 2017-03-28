namespace PodFul.Library.IntegrationTests

open System.IO
open System.Reflection
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library

type ImageResolver_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\ImageResolver_IntergrationTests\";

    let createImageResolver imageDirectory defaultImagePath renameFunction =
        let imageResolver = ImageResolver(imageDirectory, defaultImagePath, false)
        imageResolver.RenameFunction <- renameFunction
        imageResolver :> IImageResolver

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Local path not set and URL path is set so file is downloaded and file path is returned``() =
        
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = ""
        let expectedPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"
        let imageResolver = createImageResolver imageDirectory defaultImagePath renameFunction

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        // Act
        let resultPath = imageResolver.GetName localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, expectedPath)
        Assert.AreEqual(File.Exists(expectedPath), true);

    [<Test>]
    member public this.``Local path set and file exists so local path is returned``()=
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"
        let imageResolver = createImageResolver imageDirectory defaultImagePath renameFunction

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, localPath)
        
        // Act
        let resultPath = imageResolver.GetName localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, localPath)

    [<Test>]
    member public this.``Local path not set and URL path not set so default path is returned``() =
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = ""
        let renameFunction = fun n -> failwith "Incorrect Parameters"
        let imageResolver = createImageResolver imageDirectory defaultImagePath renameFunction

        // Act
        let resultPath = imageResolver.GetName localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, defaultImagePath)

    [<Test>]
    member public this.``Local path set but file not found locally so is downloaded and local path is returned``() =
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"
        let imageResolver = createImageResolver imageDirectory defaultImagePath renameFunction

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        // Act
        let resultPath = imageResolver.GetName localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, localPath)
        Assert.AreEqual(File.Exists(localPath), true);

    [<Test>]
    member public this.``Local name set to default and URL name not set so default name is returned``()=
        
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = defaultImagePath
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"
        let imageResolver = createImageResolver imageDirectory defaultImagePath renameFunction

        // Act
        let resultPath = imageResolver.GetName localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, localPath)

    [<Test>]
    member public this.``Local name set to default and URL name is set so file is downloaded and updated local name is returned``()=
        
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = defaultImagePath
        let expectedPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"
        let imageResolver = createImageResolver imageDirectory defaultImagePath renameFunction

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        // Act
        let resultPath = imageResolver.GetName localPath urlPath
        
        // Assert
        Assert.AreEqual(resultPath, expectedPath)
        Assert.AreEqual(File.Exists(expectedPath), true);

    [<Test>]
    member public this.``Failed download returns the default image path if options set``() =
        
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = @"C:\ImageDirectory\"
        
        let imageResolver = ImageResolver(imageDirectory, defaultImagePath, true)
        imageResolver.RenameFunction <- (fun x -> "")
        let imageResolver = imageResolver :> IImageResolver

        let actualName = imageResolver.GetName "" "Bad image url"

        Assert.AreEqual(actualName, defaultImagePath)

    [<Test>]
    member public this.``When image download fails then the posted message ends with 'FAILED'``() =
        let mutable postedMessage = ""

        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = @"C:\ImageDirectory\"

        let postMessageFunction = fun message -> postedMessage <- message

        let imageResolver = ImageResolver(imageDirectory, defaultImagePath, true) :> IImageResolver
        imageResolver.PostMessage <- new System.Action<string>(postMessageFunction)

        let actualName = imageResolver.GetName "" "Bad image url"

        Assert.AreEqual(true, postedMessage.EndsWith("FAILED\r\n"))