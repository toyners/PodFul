namespace Migration195To196

open System

type PodcastFile195 =
    {
        FileSize : Int64;
        DownloadDate : DateTime;
        ImageFileName : string;
    }