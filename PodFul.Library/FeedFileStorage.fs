namespace PodFul.Library

open System
open System.Collections.Generic
open System.IO
open System.Linq
open Jabberwocky.Toolkit.String

type FeedFileStorage(directoryPath : String) = 

    let directoryPath = directoryPath
    let feedFileExtension = ".feed"
    let mutable isopen = false
    let mutable feeds = Array.empty
    let mutable feedPaths = new Dictionary<Feed, String>()

    member private this.fileNameSubstitutions = Dictionary<String, String>(dict
                                                    [
                                                        ( "\\", "_bs_" );
                                                        ( "/", "_fs_" );
                                                        ( ":", "_c_" );
                                                        ( "*", "_a_" );
                                                        ( "?", "_q_" );
                                                        ( "\"", "_qu_" );
                                                        ( "<", "_l_" );
                                                        ( ">", "_g_" );
                                                        ( "|", "_b_" )
                                                    ])

    member private this.readLineFromFile (reader: StreamReader) : string = 
        let text = reader.ReadLine()
        match text with
        | null ->   failwith "Raw text is null."
        | "" ->     failwith "Raw text is empty."
        | _ -> text

    member private this.splitStringUsingDelimiter (text : string) : string[] = text.Split('|')

    member private this.verifyFields (fields: string[]) : string[] =
        if fields = null then
            failwith "Fields array is null."
        else if fields.Length = 0 then
            failwith "Fields array is empty."
        else if fields.Length < 7 then
            failwith ("Fields array only has " + fields.Length.ToString() + " field(s).")
        else
            fields

    member private this.getPodcastFromFile (reader: StreamReader) = 
        match reader.EndOfStream with
        | true -> None
        | _ ->
              // Create fields array using line read from reader.
              let fields = this.readLineFromFile reader |> this.splitStringUsingDelimiter |> this.verifyFields

              // Create the podcast record.
              let podcast = 
                {
                    Title = fields.[0]
                    PubDate = DateTime.Parse(fields.[1])
                    URL = fields.[2]
                    FileSize = Int64.Parse(fields.[3])
                    Description = fields.[4]
                    DownloadDate = DateTime.Parse(fields.[5])
                    ImageFileName = fields.[6]
                }

              // Set the threaded state to be the XML reader.
              Some(podcast, reader)

    member private this.getImageFileName (fields : string[]) : string =
        if fields.Length > 5 then
            fields.[5]
        else
            String.Empty

    member private this.getDateTimeAt (fields : string[], index) : DateTime =
        if fields.Length > index then
            DateTime.Parse(fields.[index])
        else
            DateTime.MinValue

    member private this.readFeedFromFile (filePath : String) : Feed =
        
        use reader = new StreamReader(filePath)
        let fields =  this.readLineFromFile reader |> this.splitStringUsingDelimiter

        { 
            Title = fields.[0]
            Website = fields.[1]
            Directory = fields.[2]
            URL = fields.[3]
            Description = fields.[4]
            ImageFileName = this.getImageFileName fields
            CreationDateTime = this.getDateTimeAt(fields, 6)
            UpdatedDateTime = this.getDateTimeAt(fields, 7)
            Podcasts = List.unfold this.getPodcastFromFile (reader) |> List.toArray
        }

    member private this.writeFeedToFile (feed : Feed) (filePath : string) : unit =
        
        use writer = new StreamWriter(filePath)

        writer.WriteLine(feed.Title + "|" + feed.Website + "|" + feed.Directory + "|" + feed.URL + "|" + feed.Description + "|" + feed.ImageFileName + "|" + feed.CreationDateTime.ToString() + "|" + feed.UpdatedDateTime.ToString() + "|");

        for podcast in feed.Podcasts do
            writer.WriteLine(podcast.Title + "|" + 
                podcast.PubDate.ToString() + "|" + 
                podcast.URL + "|" + 
                podcast.FileSize.ToString() + "|" +
                podcast.Description + "|" +
                podcast.DownloadDate.ToString() + "|" +
                podcast.ImageFileName + "|")

    interface IFeedStorage with

        member this.Feeds with get() =
                            match isopen with
                            | true -> feeds
                            | _ -> null

        member this.IsOpen with get() = isopen

        member this.Add (feed : Feed) =
            
            if feeds.Contains(feed) then
                failwith "Feed already in storage."
            
            let filePath = directoryPath + 
                           feed.Title.Substitute(this.fileNameSubstitutions) + "_" +
                           System.Guid.NewGuid().ToString() + 
                           feedFileExtension;

            this.writeFeedToFile feed filePath
            feeds <- Array.append feeds [|feed|]
            feedPaths.Add(feed, filePath)
                
        member this.Close() =
            feeds <- Array.empty
            feedPaths.Clear()
            isopen <- false

        member this.Open () = 
            
            feeds <-
                [|for filePath in Directory.GetFiles(directoryPath, "*" + feedFileExtension, SearchOption.TopDirectoryOnly) do
                    let feed = this.readFeedFromFile filePath
                    feedPaths.Add(feed, filePath)
                    yield feed
                |]

            isopen <- true

        member this.Remove (feed : Feed) = 

            let feedInStorage f = (f = feed)
            let feedIsInStorage = Array.exists feedInStorage feeds
            if feedIsInStorage <> true then
                failwith "Feed cannot be removed because it cannot be found in storage."

            let filePath = feedPaths.[feed]
            File.Delete(filePath)

            let allOtherFeeds f = (f <> feed)
            feeds <- Array.filter allOtherFeeds feeds
            feedPaths.Remove(feed) |> ignore

        member this.Update (feed : Feed) =
            let filePath = feedPaths.[feed]
            let oldFilePath = filePath + ".old"

            // Move the current file to be the old file. Delete any existing old file. 
            if File.Exists(oldFilePath) then
                File.Delete(oldFilePath)
            File.Move(filePath, oldFilePath)

            this.writeFeedToFile feed filePath |> ignore

            // Update the feed object in the array
            let equalsFeed f = (f = feed)
            let index = Array.findIndex equalsFeed feeds
            Array.set feeds index feed
            
    member public this.Storage () =
            this :> IFeedStorage
