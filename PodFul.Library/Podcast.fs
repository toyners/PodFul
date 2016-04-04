namespace PodFul.Library

open System

type Podcast =
    {
        Title : string;
        Description : string;
        URL : string;
        FileSize : Int64;
        PubDate : DateTime;
    }