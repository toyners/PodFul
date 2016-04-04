namespace PodFul.Library

open System

type Channel = 
    {
        Title : string;
        Description : string;
        Website : string;
        Directory : string;
        Feed : string;
        Podcasts : Podcast[];
    }