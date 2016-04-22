namespace PodFul.Library

open System
open System.IO
open System.Net
open System.Xml.Linq
open FSharp.Data

module public FeedFunctions =

    let private xn name = XName.Get(name)

    // This implementation of the dynamic operator ? returns the child element from the parent that matches the name.
    let private (?) (parent : XElement) name : XElement = parent.Element(xn name)

    let private getFileSizeFromItem (element : XElement) : Int64 =
        let attribute = element.Attribute(xn "length")
        if attribute = null || attribute.Value = null || attribute.Value = "" then
            -1L
        else
            attribute.Value |> Int64.Parse

    let private removeTimeZoneAbbreviationsFromDateTimeString (value : string) : string =
        let mutable length = value.Length;
        while Char.IsLetter(value.[length - 1]) do
            length <- length - 1
        
        value.Substring(0, length)

    let private getPubDateFromItem (element : XElement) : DateTime =
        let pubDateElement = element?pubDate
        if pubDateElement = null || pubDateElement.Value = "" then
            DateTime.MinValue
        else
            pubDateElement.Value |> removeTimeZoneAbbreviationsFromDateTimeString |> DateTime.Parse

    let private cleanText (text: string) : string = 
        // Replace line breaks and multiple spaces with single spaces
        let mutable fixedText = text.Replace("\r\n", " ").Replace("\n", " ")
        while fixedText.IndexOf("  ") <> -1 do 
            fixedText <- fixedText.Replace("  ", " ")

        // Replace HTML code for right single quotation mark
        fixedText <- fixedText.Replace("&#8217;", "'")

        fixedText

    let private getDescriptionFromItem (element : XElement) : string =
        let descriptionElement = element?description
        if descriptionElement = null then
            ""
        else
            cleanText descriptionElement.Value

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
        else if fields.Length < 7 then
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
                    PubDate = DateTime.Parse(fields.[1])
                    URL = fields.[2]
                    FileSize = Int64.Parse(fields.[3])
                    Description = fields.[4]
                    FirstDownloadDate = DateTime.Parse(fields.[5])
                    LatestDownloadDate = DateTime.Parse(fields.[6])
                }

              // Set the threaded state to be the XML reader.
              Some(podcast, reader)

    let private createPodcastArrayFromDocument (document: XDocument) =

        [for element in document.Descendants(xn "item") do
            let titleElement = element?title
            let enclosureElement = element?enclosure

            if titleElement <> null && enclosureElement <> null then
                let urlAttribute = enclosureElement.Attribute(xn "url")
                
                if urlAttribute <> null then
                    let url = urlAttribute.Value

                    if url <> null && url <> "" then
                        yield {
                            Title = titleElement.Value
                            Description = getDescriptionFromItem element
                            PubDate = getPubDateFromItem element
                            URL = url
                            FileSize = getFileSizeFromItem enclosureElement
                            FirstDownloadDate = DateTime.MinValue
                            LatestDownloadDate = DateTime.MinValue
                        }
          ] |> List.toArray

    let public DownloadDocument(url) : XDocument = 
        try
            let webClient = new WebClient()
            webClient.Headers.Add("user-agent", "Podful Podcatcher")
            let data = webClient.DownloadString(Uri(url))
            XDocument.Parse(data)
        with
        | :? System.Net.WebException as webex ->
             use streamReader = new StreamReader(webex.Response.GetResponseStream())
             let errorLogFilePath = Path.GetTempPath() + "PodFul.log"
             use streamWriter = new StreamWriter(errorLogFilePath)
             let responseText = streamReader.ReadToEnd()
             streamWriter.Write(responseText)
             failwith ("Error log written to '" + errorLogFilePath + "'.")

    let public CreateFeed url directoryPath =
        let document = DownloadDocument url
        let channel = document.Element(xn "rss").Element(xn "channel")

        {
             Title = channel?title.Value
             Description = cleanText channel?description.Value
             Website = channel?link.Value
             Directory = directoryPath
             URL = url
             Podcasts = createPodcastArrayFromDocument document
        }

    let public CreatePodcastList url =
        let document = DownloadDocument url
        let channel = document.Element(xn "rss").Element(xn "channel")
        createPodcastArrayFromDocument document

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
            writer.WriteLine(podcast.Title + "|" + 
                podcast.PubDate.ToString() + "|" + 
                podcast.URL + "|" + 
                podcast.FileSize.ToString() + "|" +
                podcast.Description + "|" +
                podcast.FirstDownloadDate.ToString() + "|" + 
                podcast.LatestDownloadDate.ToString() + "|")