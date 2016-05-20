namespace PodFul.Library

open System
open System.Collections.Generic
open System.IO

type FeedFileStorage(directoryPath : String) = 

    let mutable isopen = false
    let feeds = new Dictionary<Feed, String>()
    let mutable feedKeys = [||]

    interface IFeedStorage with

        member this.Feeds with get() = 
                            match isopen with
                            | true -> feedKeys
                            | _ -> failwith "FileFeedStorage is not open"

        member this.IsOpen with get() = isopen

        member this.Add (feed : Feed) =
            let filePath = ""
            FeedFunctions.WriteFeedToFile feed filePath
            feeds.Add(feed, filePath)
            feeds.Keys.CopyTo(feedKeys, feeds.Keys.Count)

        member this.Close() =
            feeds.Clear()
            isopen <- false

        member this.Open () = 
            let filePaths = Directory.GetFiles(directoryPath, "*.txt", SearchOption.TopDirectoryOnly)
            for filePath in filePaths do
                feeds.Add(FeedFunctions.ReadFeedFromFile(filePath), filePath)
            isopen <- true

        member this.Remove (feed : Feed) = 
            let filePath = feeds.[feed]
            File.Delete(filePath)
            feeds.Remove(feed) |> ignore
            
        member this.Update (feed : Feed) =
            let filePath = feeds.[feed]
            FeedFunctions.WriteFeedToFile feed filePath |> ignore
            
    member public this.Storage () =
            this :> IFeedStorage
