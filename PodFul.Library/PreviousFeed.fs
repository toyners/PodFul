namespace PodFul.Library

open System

type PreviousFeed = 
    {
        Title : string;
        Description : string;
        Website : string;
        Directory : string;
        URL : string;
        ImageURL : string;
        ImageFileName : string;
        Podcasts : Podcast[];
        CreationDateTime : DateTime
        UpdatedDateTime : DateTime
    }
