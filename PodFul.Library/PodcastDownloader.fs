namespace PodFul.Library

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Threading

type PodcastDownloader(
                        onBeforeDownload, 
                        onSuccessfulDownload : Action<Podcast, String>, 
                        onException : Action<Podcast, Exception>, 
                        onCancelledDownload, 
                        cancellationToken, 
                        updateProgress) =

    let onBeforeDownload = onBeforeDownload
    let onSuccessfulDownload = onSuccessfulDownload
    let updateProgress = updateProgress
    let cancellationToken = cancellationToken
    let onException = onException
    let onCancelledDownload = onCancelledDownload 

    let invoke (fn : Action<Podcast>) podcast =
        if fn <> null then
            fn.Invoke podcast

        ignore

    let handleException podcast ex = 
        if onException <> null then
            onException.Invoke(podcast, ex)

    let getSizeOfDownloadedFile filePath = (new FileInfo(filePath)).Length

    let finaliseFileSizeOfPodcast (podcast : Podcast) fileSize = 
        match podcast.FileSize = fileSize with
        | false -> Podcast.SetFileSize podcast fileSize
        | _ -> podcast

    let setDownloadDateOfPodcastToNow podcast = Podcast.SetDownloadDate podcast DateTime.Now

    let handleSuccess podcast filePath =
        if onSuccessfulDownload <> null then
            onSuccessfulDownload.Invoke(podcast, filePath)
    
        getSizeOfDownloadedFile filePath |> finaliseFileSizeOfPodcast podcast |> setDownloadDateOfPodcastToNow

    let combine x y =
        Path.Combine(x, y)

    let download directoryPath podcast (cancelToken: CancellationToken) (updateProgressFn: Action<int>) : bool = 

        invoke onBeforeDownload podcast |> ignore

        let filePath = podcast.URL.LastIndexOf('/') + 1 |> podcast.URL.Substring |> combine directoryPath

        let result =
            try
                let downloader = new FileDownloader()
                downloader.Download(podcast.URL, filePath, cancelToken, updateProgressFn)

                match cancelToken.IsCancellationRequested with
                    | true -> 
                        invoke onCancelledDownload podcast |> ignore
                        false
                    | _ ->
                        handleSuccess podcast filePath |> ignore
                        true
                
            with 
                | ex -> handleException podcast ex |> ignore; false
       
        result