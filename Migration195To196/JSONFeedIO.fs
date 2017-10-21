namespace Migration195To196

open System
open System.IO
open System.Runtime.Serialization.Json
open System.Text
open PodFul.Library

module FeedMigration =

    let ResolveFeedDifferences (previousFeed : Feed195) : Feed = 
        {
            Title = previousFeed.Title
            Description = previousFeed.Description
            Website = previousFeed.Website
            Directory = previousFeed.Directory
            URL = previousFeed.URL
            ImageURL = previousFeed.ImageURL
            ImageFileName = previousFeed.ImageFileName
            //Podcasts = previousFeed.Podcasts
            Podcasts = [||]
            CreationDateTime = previousFeed.CreationDateTime
            UpdatedDateTime = previousFeed.UpdatedDateTime
            DoScan = previousFeed.DoScan
            CompleteDownloadsOnScan = previousFeed.CompleteDownloadsOnScan
            DeliverDownloadsOnScan = previousFeed.DeliverDownloadsOnScan
        }

    let ReadFeedFromFile (filePath : string) : Feed195 =
        use reader = new StreamReader(filePath) 
        use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(reader.ReadToEnd())) 

        (new DataContractJsonSerializer(typeof<Feed>)).ReadObject(ms) :?> Feed195

    let WriteFeedToFile (filePath : string) (feed : Feed) : unit = 
        use ms = new MemoryStream()
        (new DataContractJsonSerializer(typeof<Feed>)).WriteObject(ms, feed)
    
        use writer = new StreamWriter(filePath)
        writer.Write(Encoding.Default.GetString(ms.ToArray()))

    let ProcessFile (filePath : string) : unit =
        use reader = new StreamReader(filePath) 
        use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(reader.ReadToEnd())) 

        ReadFeedFromFile filePath |>
        ResolveFeedDifferences |>
        WriteFeedToFile "" |> 
        ignore
