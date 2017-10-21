namespace Migration195To196

open System

type Feed195 = 
    {
        Title : string;
        Description : string;
        Website : string;
        Directory : string;
        URL : string;
        ImageURL : string;
        ImageFileName : string;
        Podcasts : Podcast195[];
        CreationDateTime : DateTime
        UpdatedDateTime : DateTime
        DoScan : bool
        CompleteDownloadsOnScan : bool
        DeliverDownloadsOnScan : bool
    }