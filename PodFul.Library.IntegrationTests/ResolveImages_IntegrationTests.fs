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
    member public this.``Podcast image filename set but file not found locally so is downloaded and local path is returned``() =
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

    [<Test>]
    member public this.``Podcast image files need downloading so download count function is called with correct download count``() =
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName1 = "Image1.jpg"
        let urlPath1 = workingDirectory + fileName1

        let fileName2 = "Image2.jpg"
        let urlPath2 = workingDirectory + fileName2
        let localPath2 = imageDirectory + fileName2

        let fileName3 = "Image3.jpg"
        let urlPath3 = workingDirectory + fileName3
        let localPath3 = defaultImagePath

        let fileName4 = "Image4.jpg"
        let urlPath4 = ""
        let localPath4 = imageDirectory + fileName4

        let localPath = ""
        let resolveLocalFilePathFunction = fun n -> 
            match n with
            | urlPath1 when urlPath1 = n -> fileName1
            | urlPath2 when urlPath2 = n -> fileName2
            | urlPath3 when urlPath3 = n -> fileName3
            | _ -> failwith "Incorrect Parameters"
            
        let mutable downloadCount = 0
        let reportDownloadCountFunction = fun n -> downloadCount <- n

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName1, urlPath1)
        assembly.CopyEmbeddedResourceToFile(fileName2, urlPath2)
        assembly.CopyEmbeddedResourceToFile(fileName3, urlPath3)
        assembly.CopyEmbeddedResourceToFile(fileName4, localPath4)

        // Podcast image filename not set and URL path is set so file is downloaded
        let podcast1 = createTestPodcast "" urlPath1
        // Local path set but file not found locally so file is downloaded
        let podcast2 = createTestPodcast localPath2 urlPath2
        // Podcast image filename set to default and URL path is set so file is downloaded
        let podcast3 = createTestPodcast localPath urlPath3
        // Podcast image filename set and file exists so nothing downloaded
        let podcast4 = createTestPodcast localPath4 urlPath4

        resolveImages [| podcast1; podcast2; podcast3; podcast4 |]

        // Assert
        Assert.AreEqual(3, downloadCount)

    [<Test>]
    member public this.``Podcast image files need downloading so file details of each download posted before downloading ``() =
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = workingDirectory + @"ImageDirectory\"

        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

        let fileName1 = "Image1.jpg"
        let urlPath1 = workingDirectory + fileName1

        let fileName2 = "Image2.jpg"
        let urlPath2 = workingDirectory + fileName2
        let localPath2 = imageDirectory + fileName2

        let fileName3 = "Image3.jpg"
        let urlPath3 = workingDirectory + fileName3
        let localPath3 = defaultImagePath

        let fileName4 = "Image4.jpg"
        let urlPath4 = ""
        let localPath4 = imageDirectory + fileName4

        let localPath = ""
        let resolveLocalFilePathFunction = fun n -> 
            match n with
            | urlPath1 when urlPath1 = n -> fileName1
            | urlPath2 when urlPath2 = n -> fileName2
            | urlPath3 when urlPath3 = n -> fileName3
            | _ -> failwith "Incorrect Parameters"
            
        let downloadIndexes = [||]
        let reportDownloadStartFunction = 
            fun index file -> 
                Array.append downloadIndexes index, file

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName1, urlPath1)
        assembly.CopyEmbeddedResourceToFile(fileName2, urlPath2)
        assembly.CopyEmbeddedResourceToFile(fileName3, urlPath3)
        assembly.CopyEmbeddedResourceToFile(fileName4, localPath4)

        // Podcast image filename not set and URL path is set so file is downloaded
        let podcast1 = createTestPodcast "" urlPath1
        // Local path set but file not found locally so file is downloaded
        let podcast2 = createTestPodcast localPath2 urlPath2
        // Podcast image filename set to default and URL path is set so file is downloaded
        let podcast3 = createTestPodcast localPath urlPath3
        // Podcast image filename set and file exists so nothing downloaded
        let podcast4 = createTestPodcast localPath4 urlPath4

        resolveImages [| podcast1; podcast2; podcast3; podcast4 |]

        // Assert
        Assert.AreEqual(3, downloadIndexes.Length)
        Assert.AreEqual(1, fst downloadIndexes.[0])
        Assert.AreEqual(urlPath1, snd downloadIndexes.[0])
        Assert.AreEqual(2, fst downloadIndexes.[1])
        Assert.AreEqual(urlPath2, snd downloadIndexes.[1])
        Assert.AreEqual(3, fst downloadIndexes.[2])
        Assert.AreEqual(urlPath3, snd downloadIndexes.[2])

