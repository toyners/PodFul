namespace PodFul.Library

open System
open System.IO
open System.Threading

module public ImageFunctions =

    let needsToDownloadImage (localImagePath : string) (onlineImagePath : string) (defaultImagePath : string) : bool =
        let gotLocalImagePath = String.IsNullOrEmpty(localImagePath) = false
        let gotOnlineImagePath = String.IsNullOrEmpty(onlineImagePath) = false

        ((gotLocalImagePath = false || localImagePath = defaultImagePath) && gotOnlineImagePath) ||
            (gotLocalImagePath && File.Exists(localImagePath) = false)

    let resolveImages 
        (podcasts : Podcast[]) 
        (localImageDirectory : string)
        (defaultImagePath : string) 
        resolveLocalFilePathFunction 
        totalDownloadsRequiredNotificationFunction
        startDownloadNotificationFunction 
        skippedDownloadNotificationFunction
        completedDownloadNotificationFunction 
        failedDownloadNotificationFunction 
        (cancelToken : CancellationToken) =

        // Get count of images that need downloading for collection of podcasts
        let mutable index = 0
        let mutable downloadTotal = 0
        while index < podcasts.Length do
            cancelToken.ThrowIfCancellationRequested()

            let podcast = podcasts.[index]

            if needsToDownloadImage podcast.FileDetails.ImageFileName podcast.ImageURL defaultImagePath then
                downloadTotal <- downloadTotal + 1

            index <- index + 1

        totalDownloadsRequiredNotificationFunction downloadTotal

        index <- 0
        let mutable downloadNumber = 0
        let imageDownloader = new FileDownloader()
        let imagesDownloaded = new System.Collections.Generic.Dictionary<string, string>()
        while index < podcasts.Length do
            cancelToken.ThrowIfCancellationRequested()

            let podcast = podcasts.[index]
            
            if needsToDownloadImage podcast.FileDetails.ImageFileName podcast.ImageURL defaultImagePath then
                downloadNumber <- downloadNumber + 1
                
                if imagesDownloaded.ContainsKey(podcast.ImageURL) then
                    skippedDownloadNotificationFunction downloadNumber podcast.ImageURL
                    podcast.SetImageFileName imagesDownloaded.[podcast.ImageURL]
                else
                    let localImageFileName = resolveLocalFilePathFunction podcast.ImageURL
                    let localImagePath = Path.Combine(localImageDirectory, localImageFileName)
                    imagesDownloaded.Add(podcast.ImageURL, localImagePath) |> ignore
                
                    try
                        startDownloadNotificationFunction downloadNumber podcast.ImageURL    
                        imageDownloader.Download(podcast.ImageURL, localImagePath, System.Threading.CancellationToken.None, null) |> ignore
                        podcast.SetImageFileName localImagePath
                        completedDownloadNotificationFunction podcast.ImageURL
                    with
                    | _ as ex ->
                        podcast.SetImageFileName defaultImagePath
                        failedDownloadNotificationFunction podcast.ImageURL ex
            else if System.String.IsNullOrEmpty(podcast.FileDetails.ImageFileName) then
                podcast.SetImageFileName defaultImagePath

            index <- index + 1
        