namespace PodFul.Library

open System
open System.Xml
open System.IO
open FSharp.Data

module public ChannelFunctions =

    let verifyMove nodeName result = 
        match result with
        | false -> failwith ("'" + nodeName + "' node not found.")
        | _ -> ignore

    let moveToDescendant (reader : XmlReader, nodeName) =
        reader.ReadToDescendant(nodeName) |> verifyMove nodeName

    let moveToFollowing (reader: XmlReader, nodeName) =
        reader.ReadToFollowing(nodeName) |> verifyMove nodeName

    let moveToNextSibling (reader: XmlReader, nodeName) = 
        reader.ReadToNextSibling(nodeName) |> verifyMove nodeName

    (*let getStringFromNode (reader : XmlReader, nodeName) = 
        moveToNextSibling(reader,nodeName) |> ignore
        reader.ReadElementContentAsString();*)

    let getDataFromNode moveFn readFn = 
        moveFn |> ignore
        readFn()

    let getPubDateFromNode (reader: XmlReader) =
        reader.ReadToNextSibling("pubDate") |> verifyMove "pubDate" |> ignore
        System.DateTime.Parse(reader.ReadElementContentAsString())

    let getAttribute (reader: XmlReader, attributeName: string) = 
        let content = reader.GetAttribute(attributeName)
        match content with 
        | null -> failwith ("'" + attributeName + "' attribute not found.")
        | _ -> content

    let getFileDetails (reader: XmlReader) =
        reader.ReadToNextSibling("enclosure") |> verifyMove "enclosure" |> ignore
        
        let url = getAttribute(reader, "url")
        let length = getAttribute(reader, "length")

        (url, Int64.Parse(length))

    let getPodcastRecordsFromRSS (reader: XmlReader) =
        match reader.ReadToNextSibling("item") with
        | false -> None
        | _ ->  
                // Create a sub reader to read the item nodes.
                let itemReader = reader.ReadSubtree();
                itemReader.Read() |> ignore

                // Create the podcast record
                let podcastRecord = 
                    {
                        Title = getDataFromNode (moveToDescendant(itemReader, "title")) itemReader.ReadElementContentAsString 
                        PubDate = getPubDateFromNode itemReader
                        Description = getDataFromNode (moveToNextSibling(itemReader, "description")) itemReader.ReadElementContentAsString
                        URL = null
                        FileSize = 0L
                    }

                // Read the url and file length and set these properties on the podcast record.
                let url, fileSize = getFileDetails itemReader
                let podcastRecord = { podcastRecord with URL = url; FileSize = fileSize }

                // Close the sub reader. This will set the parent reader to the end element of the item node ready
                // to move onto the next item.
                itemReader.Close()

                // Set the threaded state to be the XML reader
                Some(podcastRecord, reader)

    let getChannelRecordFromRSS (rssURL : string, directory : string, fileName : string) = 
    
        use stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)
        use reader = XmlReader.Create(stream)
        moveToFollowing (reader, "channel") |> ignore
         
        { 
            Title = getDataFromNode (moveToDescendant(reader, "title")) reader.ReadElementContentAsString
            Website = getDataFromNode (moveToFollowing(reader, "link")) reader.ReadElementContentAsString
            Description = getDataFromNode (moveToFollowing(reader, "description")) reader.ReadElementContentAsString
            Directory = directory
            Feed = rssURL
            Podcasts =  List.unfold getPodcastRecordsFromRSS (reader) |> List.toArray
        }