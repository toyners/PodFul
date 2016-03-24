namespace PodFul.Library

open System

type ChannelRecord = 
    {
        Title : string;
        Description : string;
        Website : string;
        Directory : string;
        Feed : string;
        Podcasts : PodcastRecord[];
    }