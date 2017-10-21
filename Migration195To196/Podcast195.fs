namespace Migration195To196

open System

type Podcast195 =
    {
        Title : string;
        Description : string;
        URL : string;
        ImageURL : string;
        PubDate : DateTime;
        mutable FileDetails : PodcastFile195;
    }