namespace PodFul.Library.IntegrationTests

open System.IO
open System.Reflection
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library

type ResolveImages_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\ResolveImages_IntergrationTests\";

    let resolveImages (podcasts : Podcast[]) =
        // Get count of images that need downloading for collection of podcasts

        // If not downloading images use default image

        // iterate over podcasts and
        // resolve name to local file name
        // if local file exists then use it
        // if local file does not exist then
        //      try download image
        //      if allowed return local file name
        //      if not allowed then
        //          try use default image
        //          if passed in return default image name
        //          if not passed in set name to blank
        //      if allowed and failed then
        //          Log
        //          try use default image        
        //          if passed in set default image name
        //          if not passed in return blank
        ()

    let createTestPodcast imageFileName imageURL =
        {
            Title = ""
            Description = ""
            URL = ""
            ImageURL = imageURL
            PubDate = System.DateTime.Now
            FileDetails =
                {
                    FileSize = 0L
                    DownloadDate = System.DateTime.Now
                    ImageFileName = imageFileName
                }
        }

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)

    [<Test>]
    member public this.``Podcast image filename not set and URL path is set so file is downloaded and podcast image path is updated``() =
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = ""
        let expectedPath = imageDirectory + fileName
        let resolveLocalFilePathFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)

        let podcast = createTestPodcast localPath urlPath 

        resolveImages [|podcast|]

        // Assert
        Assert.AreEqual(expectedPath, podcast.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(expectedPath))

    [<Test>]
    member public this.``Podcast image filename set and file exists so podcast image filename is not changed``()=
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, localPath)

        let podcast = createTestPodcast localPath urlPath 
        
        // Act
        resolveImages [|podcast|]
        
        // Assert
        Assert.AreEqual(localPath, podcast.FileDetails.ImageFileName)

    [<Test>]
    member public this.``Podcast image filename not set and URL path not set so default image filename is used``() =
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = ""
        let renameFunction = fun n -> failwith "Incorrect Parameters"

        let podcast = createTestPodcast localPath urlPath 

        // Act
        resolveImages [|podcast|]
        
        // Assert
        Assert.AreEqual(defaultImagePath, podcast.FileDetails.ImageFileName)

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

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        let podcast = createTestPodcast localPath urlPath 

        // Act
        resolveImages [|podcast|]
        
        // Assert
        Assert.AreEqual(localPath, podcast.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(localPath))

    [<Test>]
    member public this.``Podcast image filename set to default and URL path is set so file is downloaded and podcast image path is updated``()=
        
        // Arrange
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = defaultImagePath
        let expectedPath = imageDirectory + fileName
        let renameFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)

        let podcast = createTestPodcast localPath urlPath
        
        // Act
        resolveImages [|podcast|]
        
        // Assert
        Assert.AreEqual(expectedPath, podcast.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(expectedPath))

    [<Test>]
    member public this.``Image download fails so default image filename is used``() =
        
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = @"C:\ImageDirectory\"
        
        let podcast = createTestPodcast "" "Bad image url"

        resolveImages [|podcast|]

        Assert.AreEqual(defaultImagePath, podcast.FileDetails.ImageFileName)
