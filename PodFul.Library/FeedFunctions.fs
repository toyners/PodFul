namespace PodFul.Library

open System
open System.IO
open System.Net
open System.Xml.Linq
open FSharp.Data

module public FeedFunctions =

    let private xn name = XName.Get(name)

    // This implementation of the dynamic operator ? returns the child element from the parent that matches the name.
    let private (?) (parent : XElement) name : XElement = 
        let child = parent.Element(xn name)
        match child with
        | null -> failwith ("Element '" + name + "' not found in '" + parent.Name.LocalName + "'")
        | _ -> child;

    let private getAttributeValue (element: XElement) name : string = 
        let attribute = element.Attribute(xn name)
        match attribute with 
        | null -> failwith ("Atributr '" + name + "' not found in '" + element.Name.LocalName + "'")
        | _ -> attribute.Value

    let private readLineFromFile (reader: StreamReader) : string = 
        let text = reader.ReadLine()
        match text with
        | null ->   failwith "Raw text is null."
        | "" ->     failwith "Raw text is empty."
        | _ -> text

    let private verifyFields (fields: string[]) : string[] =
        if fields = null then
            failwith "Fields array is null."
        else if fields.Length = 0 then
            failwith "Fields array is empty."
        else if fields.Length < 5 then
            failwith ("Fields array only has " + fields.Length.ToString() + " field(s).")
        else
            fields

    let private splitStringUsingCharacter (delimiter: Char) (text : string) : string[] = text.Split(delimiter)

    let private getPodcastFromFile (reader: StreamReader) = 
        match reader.EndOfStream with
        | true -> None
        | _ ->
              // Create fields array using line read from reader.
              let fields = readLineFromFile reader |> splitStringUsingCharacter '|' |> verifyFields

              // Create the podcast record.
              let podcast = 
                {
                    Title = fields.[0]
                    PubDate = System.DateTime.Parse(fields.[1])
                    URL = fields.[2]
                    FileSize = Int64.Parse(fields.[3])
                    Description = fields.[4]
                }

              // Set the threaded state to be the XML reader.
              Some(podcast, reader)

    let public DownloadDocument(url) : XDocument = 
        let webClient = new WebClient()
        let data = webClient.DownloadString(Uri(url))
        XDocument.Parse(data)

    let public CreateFeed url directoryPath =
        let document = DownloadDocument url
        let channel = document.Element(xn "rss").Element(xn "channel")

        {
             Title = channel?title.Value
             Description = channel?description.Value
             Website = channel?link.Value
             Directory = directoryPath
             URL = url
             Podcasts = [ for element in document.Descendants(xn "item") do
                            yield {
                                Title = element?title.Value
                                Description = element?description.Value
                                PubDate = element?pubDate.Value |> DateTime.Parse
                                URL = getAttributeValue element?enclosure "url"
                                FileSize = getAttributeValue element?enclosure "length" |> Int64.Parse
                            }] |> List.toArray
        }

    let public ReadFeedFromFile(filePath : string) : Feed =
        
        use reader = new StreamReader(filePath)
        let fields =  reader.ReadLine() |> splitStringUsingCharacter '|' 

        { 
            Title = fields.[0]
            Website = fields.[1]
            Directory = fields.[2]
            URL = fields.[3]
            Description = fields.[4]
            Podcasts = List.unfold getPodcastFromFile (reader) |> List.toArray
        }

    let public WriteFeedToFile (feed : Feed) (filePath : string) : unit =
        
        use writer = new StreamWriter(filePath)

        writer.WriteLine(feed.Title + "|" + feed.Website + "|" + feed.Directory + "|" + feed.URL + "|" + feed.Description);

        for podcast in feed.Podcasts do
            writer.WriteLine(podcast.Title + "|" + podcast.PubDate.ToString() + "|" + podcast.URL + "|" + podcast.FileSize.ToString() + "|" + podcast.Description)