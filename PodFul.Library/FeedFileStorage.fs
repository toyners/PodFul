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
                            | true -> feeds
                            | _ -> null

        member this.IsOpen with get() = isopen

        member this.Add (feed : Feed) =
            
            if feeds.Contains(feed) then
                failwith "Feed already in storage."

            let filePath = directoryPath + feeds.Length.ToString() + "_" + feed.Title.Substitute(this.fileNameSubstitutions) + feedFileExtension;
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
                    let feed = FeedFunctions.ReadFeedFromFile filePath
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
            File.Move(filePath, oldFilePath)
            this.writeFeedToFile feed filePath |> ignore

            let equalsFeed f = (f = feed)

            let index = Array.findIndex equalsFeed feeds
            Array.set feeds index feed
            
    member public this.Storage () =
            this :> IFeedStorage
