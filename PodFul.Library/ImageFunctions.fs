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

    let nextImageFileName (urlPath : string) =
        let mutable name = Guid.NewGuid().ToString()
        if urlPath.EndsWith(".jpg", true, null) then
            name <- name + ".jpg"
        name

    let resolvePodcastImages 
        (podcasts : Podcast[]) 
        (localImageDirectory : string)
        (defaultImagePath : string) 
        resolveLocalFilePathFunction 
        (totalDownloadsRequiredNotificationFunction : System.Action<int>)
        (startDownloadNotificationFunction : System.Action<int, string>)
        (skippedDownloadNotificationFunction : System.Action<int, string>)
        (completedDownloadNotificationFunction : System.Action<int, string>)
        (failedDownloadNotificationFunction : System.Action<string, System.Exception>)
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

        if totalDownloadsRequiredNotificationFunction <> null then
            totalDownloadsRequiredNotificationFunction.Invoke downloadTotal

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
                    if skippedDownloadNotificationFunction <> null then
                        skippedDownloadNotificationFunction.Invoke(downloadNumber, podcast.ImageURL)
                    podcast.SetImageFileName imagesDownloaded.[podcast.ImageURL]
                else
                    let localImageFileName = resolveLocalFilePathFunction podcast.ImageURL
                    let localImagePath = Path.Combine(localImageDirectory, localImageFileName)
                    imagesDownloaded.Add(podcast.ImageURL, localImagePath) |> ignore
                
                    try
                        if startDownloadNotificationFunction <> null then
                            startDownloadNotificationFunction.Invoke(downloadNumber, podcast.ImageURL)    
                        imageDownloader.Download(podcast.ImageURL, localImagePath, System.Threading.CancellationToken.None, null) |> ignore
                        podcast.SetImageFileName localImagePath
                        if completedDownloadNotificationFunction <> null then
                            completedDownloadNotificationFunction.Invoke(downloadNumber, podcast.ImageURL)
                    with
                    | _ as ex ->
                        podcast.SetImageFileName defaultImagePath
                        if failedDownloadNotificationFunction <> null then
                            failedDownloadNotificationFunction.Invoke(podcast.ImageURL, ex)
            else if System.String.IsNullOrEmpty(podcast.FileDetails.ImageFileName) then
                podcast.SetImageFileName defaultImagePath

            index <- index + 1

    let resolveImagesForPodcasts
        (podcasts : Podcast[]) 
        (localImageDirectory : string)
        (defaultImagePath : string) 
        (totalDownloadsRequiredNotificationFunction : System.Action<int>)
        (startDownloadNotificationFunction : System.Action<int, string>)
        (skippedDownloadNotificationFunction : System.Action<int, string>)
        (completedDownloadNotificationFunction : System.Action<int, string>)
        (failedDownloadNotificationFunction : System.Action<string, System.Exception>)
        (cancelToken : CancellationToken) =

        resolvePodcastImages 
            podcasts 
            localImageDirectory 
            defaultImagePath 
            nextImageFileName
            totalDownloadsRequiredNotificationFunction
            startDownloadNotificationFunction
            skippedDownloadNotificationFunction
            completedDownloadNotificationFunction
            failedDownloadNotificationFunction
            cancelToken

    let resolveFeedImage
        (feed : Feed)
        (localImageDirectory : string)
        (defaultImagePath : string) 
        resolveLocalFilePathFunction 
        (failedDownloadNotificationFunction : Action<string, System.Exception>) = 

        if needsToDownloadImage feed.ImageFileName feed.ImageURL defaultImagePath then
            let imageDownloader = new FileDownloader()
            let localImageFileName = resolveLocalFilePathFunction feed.ImageURL
            let localImagePath = Path.Combine(localImageDirectory, localImageFileName)
            try
                imageDownloader.Download(feed.ImageURL, localImagePath, System.Threading.CancellationToken.None, null) |> ignore
                Feed.SetImageFileName feed localImagePath 
            with
            | _ as ex ->
                if failedDownloadNotificationFunction <> null then
                    failedDownloadNotificationFunction.Invoke(feed.ImageURL, ex)
                Feed.SetImageFileName feed defaultImagePath 
        else
            feed

    let resolveImageForFeed
        (feed : Feed)
        (localImageDirectory : string)
        (defaultImagePath : string) 
        (failedDownloadNotificationFunction : Action<string, System.Exception>) = 
    
        resolveFeedImage
            feed
            localImageDirectory
            defaultImagePath
            nextImageFileName
            failedDownloadNotificationFunction 
