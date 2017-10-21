namespace Migration195To196

open System
open System.IO
open System.Runtime.Serialization.Json
open System.Text
open PodFul.Library

module FeedMigration =

    let resolvePodcastDifferences (previousPodcast : Podcast195) : Podcast = 
        {
            Title = previousPodcast.Title
            Description = previousPodcast.Description
            URL = previousPodcast.URL
            ImageURL = previousPodcast.ImageURL
            PubDate = previousPodcast.PubDate
            FileDetails = 
            {
                FileName = ""
                FileSize = previousPodcast.FileDetails.FileSize
                DownloadDate = previousPodcast.FileDetails.DownloadDate
                ImageFileName = previousPodcast.FileDetails.ImageFileName
            }
        }

    let resolveFeedDifferences (previousFeed : Feed195) : Feed = 
        {
            Title = previousFeed.Title
            Description = previousFeed.Description
            Website = previousFeed.Website
            Directory = previousFeed.Directory
            URL = previousFeed.URL
            ImageURL = previousFeed.ImageURL
            ImageFileName = previousFeed.ImageFileName
            Podcasts = Array.map resolvePodcastDifferences previousFeed.Podcasts
            CreationDateTime = previousFeed.CreationDateTime
            UpdatedDateTime = previousFeed.UpdatedDateTime
            DoScan = previousFeed.DoScan
            CompleteDownloadsOnScan = previousFeed.CompleteDownloadsOnScan
            DeliverDownloadsOnScan = previousFeed.DeliverDownloadsOnScan
        }

    let readFeedFromFile (filePath : string) : Feed195 =
        use reader = new StreamReader(filePath) 
        use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(reader.ReadToEnd())) 

        (new DataContractJsonSerializer(typeof<Feed195>)).ReadObject(ms) :?> Feed195

    let writeFeedToFile (filePath : string) (feed : Feed) : unit = 
        use ms = new MemoryStream()
        (new DataContractJsonSerializer(typeof<Feed>)).WriteObject(ms, feed)
    
        use writer = new StreamWriter(filePath)
        writer.Write(Encoding.Default.GetString(ms.ToArray()))

    let processFile (filePath : string) : unit =
        readFeedFromFile filePath |>
        resolveFeedDifferences |>
        writeFeedToFile filePath |> 
        ignore
