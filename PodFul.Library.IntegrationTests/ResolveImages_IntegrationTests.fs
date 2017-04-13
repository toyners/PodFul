namespace PodFul.Library.IntegrationTests

open System
open System.IO
open System.Reflection
open Jabberwocky.Toolkit.Assembly
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.Library

type ResolveImages_IntegrationTests() =

    let workingDirectory = @"C:\Projects\PodFul\PodFul.Library.IntegrationTests\Test\ResolveImages_IntergrationTests\";
    let imageDirectory = workingDirectory + @"ImageDirectory\"
    let defaultImagePath = @"C:\DefaultImage.jpg"

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

    let runResolveImagesWithoutFeedBack (podcasts : Podcast[]) (localImageDirectory : string) (defaultImagePath : string) resolveLocalFilePathFunction =
        ImageFunctions.resolveImages 
            podcasts 
            localImageDirectory
            defaultImagePath
            resolveLocalFilePathFunction
            (fun n -> ()) 
            (fun n s -> ())
            (fun n s -> failwith "Should not be called")
            (fun s -> ())
            (fun s e -> ())

    let createResolveLocalImageNameFunction urlPath fileName = 
        fun n -> if n = urlPath then fileName else failwith "Unexpected parameter: " + urlPath

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory)
        DirectoryOperations.EnsureDirectoryIsEmpty(imageDirectory)

    [<Test>]
    member public this.``Podcast image filename not set and URL path is set so file is downloaded and podcast image path is updated``() =

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = ""
        let expectedPath = imageDirectory + fileName

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)

        let podcast = createTestPodcast localPath urlPath 

        runResolveImagesWithoutFeedBack [|podcast|] imageDirectory null (createResolveLocalImageNameFunction urlPath fileName)

        // Assert
        Assert.AreEqual(expectedPath, podcast.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(expectedPath))

    [<Test>]
    member public this.``Podcast image filename set and file exists so podcast image filename is not changed``()=
        // Arrange

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = imageDirectory + fileName

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, localPath)

        let podcast = createTestPodcast localPath urlPath 
        
        // Act
        ImageFunctions.resolveImages
            [|podcast|] 
            imageDirectory
            null 
            (fun s -> failwith "Should not be called")
            (fun n -> ())
            (fun n s -> failwith "Should not be called")
            (fun n s -> failwith "Should not be called")
            (fun s -> failwith "Should not be called")
            (fun s e -> failwith "Should not be called")

        // Assert
        Assert.AreEqual(localPath, podcast.FileDetails.ImageFileName)

    [<Test>]
    member public this.``Podcast image filename not set and URL path not set so default image filename is used``() =

        let fileName = "Image.jpg"
        let urlPath = ""
        let localPath = ""
        let resolveLocalFilePathFunction = fun n -> failwith "Should not be called"

        let podcast = createTestPodcast localPath urlPath 

        // Act
        runResolveImagesWithoutFeedBack [|podcast|] imageDirectory defaultImagePath resolveLocalFilePathFunction

        // Assert
        Assert.AreEqual(defaultImagePath, podcast.FileDetails.ImageFileName)

    [<Test>]
    member public this.``Podcast image filename set but file not found locally so is downloaded and local path is returned``() =

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = imageDirectory + fileName

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)
        
        let podcast = createTestPodcast localPath urlPath 

        // Act
        runResolveImagesWithoutFeedBack [|podcast|] imageDirectory null (createResolveLocalImageNameFunction urlPath fileName)
        
        // Assert
        Assert.AreEqual(localPath, podcast.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(localPath))

    [<Test>]
    member public this.``Podcast image filename set to default and URL path is set so file is downloaded and podcast image path is updated``()=

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName
        let localPath = defaultImagePath
        let expectedPath = imageDirectory + fileName

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)

        let podcast = createTestPodcast localPath urlPath
        
        // Act
        runResolveImagesWithoutFeedBack [|podcast|] imageDirectory null (createResolveLocalImageNameFunction urlPath fileName)
        
        // Assert
        Assert.AreEqual(expectedPath, podcast.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(expectedPath))

    [<Test>]
    member public this.``Image download fails so default image filename is used``() =
        
        let fileName = "Image.jpg"
        let podcast = createTestPodcast "" "Bad image url"

        runResolveImagesWithoutFeedBack [|podcast|] imageDirectory defaultImagePath (fun n -> fileName)

        Assert.AreEqual(defaultImagePath, podcast.FileDetails.ImageFileName)

    [<Test>]
    member public this.``Image download fails and exception message is stored``() =
        
        let fileName = "Image.jpg"
        let podcast = createTestPodcast "" "Bad image url"

        let mutable failedFile = ""
        let mutable ex = null

        ImageFunctions.resolveImages
            [|podcast|]
            imageDirectory
            defaultImagePath 
            (fun n -> fileName)
            (fun n -> ())
            (fun n s -> ())
            (fun n s -> failwith "Should not be called")
            (fun s -> ())
            (fun s e -> 
                failedFile <- s 
                ex <- e)

        Assert.AreEqual("Bad image url", failedFile)
        Assert.AreEqual(typeof<System.UriFormatException>, ex.GetType())

    [<Test>]
    member public this.``Podcast image files need downloading so download count function is called with correct download count``() =

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

        ImageFunctions.resolveImages 
            [| podcast1; podcast2; podcast3; podcast4 |] 
            imageDirectory
            null
            resolveLocalFilePathFunction
            reportDownloadCountFunction
            (fun n s -> ())
            (fun n s -> failwith "Should not be called")
            (fun s -> ())
            (fun s e -> failwith "Should not be called")

        // Assert
        Assert.AreEqual(3, downloadCount)

    [<Test>]
    member public this.``Podcast image files need downloading so file details of each download posted before downloading ``() =

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
            
        let mutable startedDownloads = [||]
        let mutable completedDownloads = [||]

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

        ImageFunctions.resolveImages 
            [| podcast1; podcast2; podcast3; podcast4 |] 
            imageDirectory
            null
            resolveLocalFilePathFunction
            (fun n -> ())
            (fun n s -> startedDownloads <- Array.append startedDownloads [|n,s|])
            (fun n s -> failwith "Should not be called")
            (fun s -> completedDownloads <- Array.append completedDownloads [|s|])
            (fun s e -> failwith "Should not be called")

        // Assert
        Assert.AreEqual(3, Array.length startedDownloads)
        Assert.AreEqual(1, fst startedDownloads.[0])
        Assert.AreEqual(urlPath1, snd startedDownloads.[0])
        Assert.AreEqual(2, fst startedDownloads.[1])
        Assert.AreEqual(urlPath2, snd startedDownloads.[1])
        Assert.AreEqual(3, fst startedDownloads.[2])
        Assert.AreEqual(urlPath3, snd startedDownloads.[2])

        Assert.AreEqual(3, completedDownloads.Length)
        Assert.AreEqual(urlPath1, completedDownloads.[0])
        Assert.AreEqual(urlPath2, completedDownloads.[1])
        Assert.AreEqual(urlPath3, completedDownloads.[2])

    [<Test>]
    member public this.``Multiple podcasts have same image file but file only downloaded once``() =

        let fileName = "Image.jpg"
        let urlPath = workingDirectory + fileName

        let expectedPath = Path.Combine(imageDirectory, fileName)

        let resolveLocalFilePathFunction = fun n -> if n = urlPath then fileName else failwith "Incorrect Parameters"

        let assembly = Assembly.GetExecutingAssembly()
        assembly.CopyEmbeddedResourceToFile(fileName, urlPath)

        let podcast1 = createTestPodcast "" urlPath
        let podcast2 = createTestPodcast "" urlPath

        let startedDownloads = [||]
        let skippedDownloads = [||]
        let completedDownloads = [||]

        ImageFunctions.resolveImages 
            [| podcast1; podcast2 |]
            imageDirectory
            null
            resolveLocalFilePathFunction
            (fun n -> ())
            (fun n s -> Array.append startedDownloads [|n,s|] |> ignore)
            (fun n s -> Array.append skippedDownloads [|n,s|] |> ignore)
            (fun s -> Array.append completedDownloads [|s|] |> ignore)
            (fun s e -> failwith "Should not be called")

        Assert.AreEqual(1, startedDownloads.Length)
        Assert.AreEqual(urlPath, snd startedDownloads.[0])

        Assert.AreEqual(1, skippedDownloads.Length)
        Assert.AreEqual(urlPath, snd skippedDownloads.[0])

        Assert.AreEqual(1, completedDownloads.Length)
        Assert.AreEqual(urlPath, snd startedDownloads.[0])

        Assert.AreEqual(expectedPath, podcast1.FileDetails.ImageFileName)
        Assert.AreEqual(expectedPath, podcast2.FileDetails.ImageFileName)
        Assert.AreEqual(true, File.Exists(expectedPath))