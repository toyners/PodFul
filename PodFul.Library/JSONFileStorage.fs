﻿namespace PodFul.Library

open System.Collections.Generic
open System.IO
open System.Linq
open Jabberwocky.Toolkit.String

type JSONFileStorage(directoryPath : string) = 

    let directoryPath = directoryPath
    let feedFileExtension = ".feed"
    
    let mutable isopen = false
    let mutable feeds = Array.empty
    let mutable feedPaths = new Dictionary<Feed, string>()

    abstract member ReadFeedFromFile: string -> Feed
    abstract member WriteFeedToFile: Feed -> string -> unit

    default this.ReadFeedFromFile filePath = JSONFeedIO.ReadFeedFromFile filePath
    default this.WriteFeedToFile feed filePath = JSONFeedIO.WriteFeedToFile feed filePath

    member private this.fileNameSubstitutions = Dictionary<string, string>(dict
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

            this.WriteFeedToFile feed filePath
            feeds <- Array.append feeds [|feed|]
            feedPaths.Add(feed, filePath)
                
        member this.Close() =
            feeds <- Array.empty
            feedPaths.Clear()
            isopen <- false

        member this.Open () = 
            
            feeds <-
                [|for filePath in Directory.GetFiles(directoryPath, "*" + feedFileExtension, SearchOption.TopDirectoryOnly) do
                    let feed = this.ReadFeedFromFile filePath
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

            this.WriteFeedToFile feed filePath |> ignore

            // Update the feed object in the array
            let equalsFeed f = (f = feed)
            let index = Array.findIndex equalsFeed feeds
            Array.set feeds index feed
            
    member public this.Storage () =
            this :> IFeedStorage
