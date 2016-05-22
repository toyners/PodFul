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
    let mutable feedKeys = [||]
    let feeds = new Dictionary<Feed, String>()

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

    member private this.writeFeedToFile (feed : Feed) (filePath : string) : unit =
        
        use writer = new StreamWriter(filePath)

        writer.WriteLine(feed.Title + "|" + feed.Website + "|" + feed.Directory + "|" + feed.URL + "|" + feed.Description);

        for podcast in feed.Podcasts do
            writer.WriteLine(podcast.Title + "|" + 
                podcast.PubDate.ToString() + "|" + 
                podcast.URL + "|" + 
                podcast.FileSize.ToString() + "|" +
                podcast.Description + "|" +
                podcast.DownloadDate.ToString() + "|")

    interface IFeedStorage with

        member this.Feeds with get() = 
                            match isopen with
                            | true -> feedKeys
                            | _ -> null

        member this.IsOpen with get() = isopen

        member this.Add (feed : Feed) =
            
            if feeds.ContainsKey(feed) then
                failwith "Feed already in storage."

            let filePath = directoryPath + feeds.Count.ToString() + "_" + feed.Title.Substitute(this.fileNameSubstitutions) + feedFileExtension;
            this.writeFeedToFile feed filePath
            feeds.Add(feed, filePath)
            feedKeys <- Enumerable.ToArray(feeds.Keys)

        member this.Close() =
            feeds.Clear()
            isopen <- false

        member this.Open () = 
            let filePaths = Directory.GetFiles(directoryPath, "*.txt", SearchOption.TopDirectoryOnly)
            for filePath in filePaths do
                feeds.Add(FeedFunctions.ReadFeedFromFile(filePath), filePath)
            isopen <- true

        member this.Remove (feed : Feed) = 
            if  feeds.ContainsKey(feed) = false then
                failwith "Feed cannot be removed because it cannot be found in storage."

            let filePath = feeds.[feed]
            File.Delete(filePath)
            feeds.Remove(feed) |> ignore
            feedKeys <- Enumerable.ToArray(feeds.Keys)

        member this.Update (feed : Feed) =
            let filePath = feeds.[feed]
            FeedFunctions.WriteFeedToFile feed filePath |> ignore
            
    member public this.Storage () =
            this :> IFeedStorage
