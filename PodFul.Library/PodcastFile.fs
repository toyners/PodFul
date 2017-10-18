namespace PodFul.Library

open System

type PodcastFile =
    {
        FileName : string;
        FileSize : Int64;
        DownloadDate : DateTime;
        ImageFileName : string;
    }
