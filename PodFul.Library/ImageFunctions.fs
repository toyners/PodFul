namespace PodFul.Library

open System
open System.IO

module public ImageFunctions =

    let needsToDownloadImage (localImagePath : string) (onlineImagePath : string) (defaultImagePath : string) : bool =
        let gotLocalImagePath = String.IsNullOrEmpty(localImagePath) = false
        let gotOnlineImagePath = String.IsNullOrEmpty(onlineImagePath) = false

        ((gotLocalImagePath = false || localImagePath = defaultImagePath) && gotOnlineImagePath) ||
            (gotLocalImagePath && File.Exists(localImagePath) = false)

    let needsToDownloadImage2 (localImagePath : string) (onlineImagePath : string) (defaultImagePath : string) : bool =
        let gotLocalImagePath = String.IsNullOrEmpty(localImagePath) = false
        let gotOnlineImagePath = String.IsNullOrEmpty(onlineImagePath) = false

        ((gotLocalImagePath = false || localImagePath = defaultImagePath) && gotOnlineImagePath) ||
            (gotLocalImagePath && File.Exists(localImagePath) = false)

    let downloadImageFile 
        sourcePath 
        destinationPath 
        defaultPath 
        startDownloadNotificationFunction
        downloadCompletedNotificationFunction
        downloadFailedNotificationFunction =
        let imageDownloader = new FileDownloader()

        startDownloadNotificationFunction destinationPath
        try
            imageDownloader.Download(sourcePath, destinationPath, System.Threading.CancellationToken.None, null) |> ignore
            downloadCompletedNotificationFunction destinationPath
            destinationPath
        with
        | _ ->
            downloadFailedNotificationFunction destinationPath  
            defaultPath

    (*let determineImageDownloadCount (podcasts : Podcast[]) (defaultImagePath : string) : int =
        let mutable index = 0
        let mutable count = 0
        while index < podcasts.Length do
            let podcast = podcasts.[index]
            let localImagePath = resolveLocalFilePathFunction podcast.ImageURL

            if needsToDownloadImage localImagePath podcasts.[index].ImageURL defaultImagePath then
                count <- count + 1
            index <- index + 1

        count*)

    let resolveImages 
        (podcasts : Podcast[]) 
        (defaultImagePath : string) 
        resolveLocalFilePathFunction 
        totalDownloadsRequiredNotificationFunction
        startDownloadNotificationFunction 
        completedDownloadNotificationFunction 
        failedDownloadNotificationFunction =

        // Get count of images that need downloading for collection of podcasts
        let mutable index = 0
        let mutable downloadTotal = 0
        while index < podcasts.Length do
            let podcast = podcasts.[index]
            let localImagePath = resolveLocalFilePathFunction podcast.ImageURL

            if needsToDownloadImage localImagePath podcast.ImageURL defaultImagePath then
                downloadTotal <- downloadTotal + 1

            index <- index + 1

        totalDownloadsRequiredNotificationFunction downloadTotal

        index <- 0
        let mutable downloadNumber = 0
        let imageDownloader = new FileDownloader()
        while index < podcasts.Length do
            let podcast = podcasts.[index]
            let localImagePath = resolveLocalFilePathFunction podcast.ImageURL
            
            if needsToDownloadImage localImagePath podcast.ImageURL defaultImagePath then
                downloadNumber <- downloadNumber + 1
                
                try
                    startDownloadNotificationFunction downloadNumber podcast.ImageURL    
                    imageDownloader.Download(podcast.ImageURL, localImagePath, System.Threading.CancellationToken.None, null) |> ignore
                    podcast.SetImageFileName localImagePath
                    completedDownloadNotificationFunction podcast.ImageURL
                with
                | _ as ex ->
                    podcast.SetImageFileName defaultImagePath
                    failedDownloadNotificationFunction podcast.ImageURL ex

            index <- index + 1
        