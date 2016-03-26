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

    let readChannelRecordFromRSSFile (rssURL : string, directory : string, fileName : string) = 
    
        use stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)
        use reader = XmlReader.Create(stream)
        moveToFollowing (reader, "channel") |> ignore
         
        { 
            Title = getDataFromNode (moveToDescendant(reader, "title")) reader.ReadElementContentAsString
            Website = getDataFromNode (moveToFollowing(reader, "link")) reader.ReadElementContentAsString
            Description = getDataFromNode (moveToFollowing(reader, "description")) reader.ReadElementContentAsString
            Directory = directory
            Feed = rssURL
            Podcasts = List.unfold getPodcastRecordsFromRSS (reader) |> List.toArray
        }

    let verifyRawText text = 
        match text with
        | null ->   failwith "Raw text is null."
        | "" ->     failwith "Raw text is empty."
        | _ -> text

    let verifyFields (fields: string[]) =
        if fields = null then
            failwith "Fields array is null."
        else if fields.Length = 0 then
            failwith "Fields array is empty."
        else
            fields

    let splitStringUsingCharacter (delimiter: Char) (text : string) : string[] = text.Split(delimiter)

    let getPodcastRecordFromFile (reader: StreamReader) = 
        match reader.EndOfStream with
        | true -> None
        | _ ->
              // Read the next line
              let text = reader.ReadLine()

              // TODO: check text exists

              // Create fields array from text
              let fields = reader.ReadLine() |> verifyRawText |> splitStringUsingCharacter '|' |> verifyFields

              // Create the podcast record
              let podcastRecord = 
                {
                    Title = fields.[0]
                    PubDate = System.DateTime.Parse(fields.[1])
                    URL = fields.[2]
                    FileSize = Int64.Parse(fields.[3])
                    Description = fields.[4]
                }

              // Set the threaded state to be the XML reader
              Some(podcastRecord, reader)

    let readChannelRecordFromFile(filePath : string) =
        
        use reader = new StreamReader(filePath)
        let fields =  reader.ReadLine() |> splitStringUsingCharacter '|' 

        { 
            Title = fields.[0]
            Website = fields.[1]
            Directory = fields.[2]
            Feed = fields.[3]
            Description = fields.[4]
            Podcasts = List.unfold getPodcastRecordFromFile (reader) |> List.toArray
        }

    let writeChannelRecordToFile (record : ChannelRecord, filePath : string) =
        
        use writer = new StreamWriter(filePath)

        writer.WriteLine(record.Title + "|" + record.Website + "|" + record.Directory + "|" + record.Feed + "|" + record.Description);
