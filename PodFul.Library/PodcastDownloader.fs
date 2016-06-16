namespace PodFul.Library

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Threading

type PodcastDownloader(
                        onBeforeDownload : Action<Podcast>, 
                        onSuccessfulDownload : Action<Podcast, String>, 
                        onException : Action<Podcast, Exception>, 
                        onCancelledDownload : Action<Podcast>, 
                        onUpdateProgress : Action<int>) =

    let onBeforeDownload = onBeforeDownload
    let onSuccessfulDownload = onSuccessfulDownload
    let onException = onException
    let onCancelledDownload = onCancelledDownload 
    let onUpdateProgress = onUpdateProgress

    let beforeDownload podcast = 
        if onBeforeDownload <> null then
            onBeforeDownload.Invoke podcast

    let handleCancel podcast =
        if onCancelledDownload <> null then
            onCancelledDownload.Invoke podcast

        podcast

    let handleException podcast ex = 
        if onException <> null then
            onException.Invoke(podcast, ex)

        podcast

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

    let download directoryPath podcast (cancelToken: CancellationToken) : Podcast = 

        beforeDownload podcast

        let filePath = podcast.URL.LastIndexOf('/') + 1 |> podcast.URL.Substring |> combine directoryPath

        try
            let downloader = new FileDownloader()
            downloader.Download(podcast.URL, filePath, cancelToken, onUpdateProgress)

            match cancelToken.IsCancellationRequested with
                | true -> 
                    handleCancel podcast
                | _ ->
                    handleSuccess podcast filePath
                
        with 
            | ex -> handleException podcast ex
       
    member this.Download(directoryPath, podcast, cancelToken) =
        download directoryPath podcast cancelToken
