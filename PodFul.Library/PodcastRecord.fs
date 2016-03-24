namespace PodFul.Library

open System

type PodcastRecord =
    {
        Title : string;
        Description : string;
        URL : string;
        FileSize : Int64;
        PubDate : DateTime;
    }