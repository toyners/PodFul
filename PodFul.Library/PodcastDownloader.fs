namespace PodFul.Library

open System

type PodcastDownloader() =

    //member public this.OnSuccessfulDownload : Action<Podcast, String> with get, set

    member public this.Download directoryPath podcasts podcastIndexes = 
        let downloader = new FileDownloader()
        ignore