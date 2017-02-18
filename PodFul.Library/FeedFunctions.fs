namespace PodFul.Library

open System
open System.IO
open System.Net
open System.Threading
open System.Xml.Linq
open FSharp.Data

module public FeedFunctions =

    // A version of DateTime.MinValue that can be serialised. Used to indicate no date time set.
    let public NoDateTime = DateTime.MinValue.ToUniversalTime()

    let private xn name = XName.Get(name)

    // This implementation of the dynamic operator ? returns the child element from the parent that matches the name.
    let private (?) (parent : XElement) name : XElement = parent.Element(xn name)

    let private getElementUsingLocalNameAndNamespace (parent : XElement) localName namespaceName = 
        let name = XName.Get(localName, namespaceName)
        parent.Element(name)

    let private removeTimeZoneAbbreviationsFromDateTimeString (value : string) : string =
        let mutable length = value.Length;
        while Char.IsLetter(value.[length - 1]) || Char.IsWhiteSpace(value.[length - 1]) do
            length <- length - 1
        
        value.Substring(0, length)

    let private getPubDateFromItem (element : XElement) : DateTime =
        let pubDateElement = element?pubDate
        if pubDateElement = null || pubDateElement.Value = String.Empty then
            NoDateTime
        else
            pubDateElement.Value |> removeTimeZoneAbbreviationsFromDateTimeString |> DateTime.Parse

    let public CleanText (dirtyText: string) : string = 
        
        // Replace line breaks and multiple spaces with single spaces
        let mutable cleanText = dirtyText.Replace("\r\n", " ").Replace("\n", " ")
        while cleanText.IndexOf("  ") <> -1 do 
            cleanText <- cleanText.Replace("  ", " ")

        // Replace known special character codes.
        cleanText <- cleanText
            .Replace("&#8217;", "'")
            .Replace("&#8230;", "...") // Actually should be the ellipsis character but I'm going to use three dots instead.

        // Remove unknown special character codes.
        cleanText <- System.Text.RegularExpressions.Regex.Replace(cleanText, "&#[0-9]*;", String.Empty)

        // Remove XML tags from the string. 
        cleanText <- System.Text.RegularExpressions.Regex.Replace(cleanText, "<.*?>", String.Empty)

        // Remove any leading and trailing whitespace.
        cleanText.Trim()

    let private getDescriptionFromItem (element : XElement) : string =
        let descriptionElement = element?description
        if descriptionElement = null then
            String.Empty
        else
            CleanText descriptionElement.Value

    let private getValueFromAttribute (element : XElement) attributeName : string = 
        let attribute = element.Attribute(xn attributeName)
        if attribute = null || attribute.Value = null || attribute.Value = String.Empty then
            String.Empty
        else
            attribute.Value

    let private getURLForItem (enclosureElement : XElement) (contentElement : XElement) =
        
        let mutable url = String.Empty
        if contentElement <> null then
            url <- getValueFromAttribute contentElement "url"
        
        if url = String.Empty && enclosureElement <> null then
            url <- getValueFromAttribute enclosureElement "url"
        
        url

    let private getFilesizeForItem (enclosureElement : XElement) (contentElement : XElement) : Int64 =
        
        let mutable size1 = -1L
        let mutable size2 = -1L
        if contentElement <> null then
            let fileSize = getValueFromAttribute contentElement "fileSize"
            if fileSize <> String.Empty then
                size1 <- (fileSize |> Int64.Parse)
        else
            let length = getValueFromAttribute enclosureElement "length"
            if length <> String.Empty then
                size2 <- (length |> Int64.Parse)

        Math.Max(size1, size2)

    let private getTitleForItem (element : XElement) : string =
        if element = null || element.Value = null || element.Value = String.Empty then
            String.Empty
        else
            element.Value.Replace("\r", String.Empty).Replace("\n", String.Empty).Replace("\t", String.Empty)

    let private getImageForChannel (channel : XElement) : string =
        match channel with
        | null -> String.Empty
        | _ ->
            let image = channel.Element(xn "image")
            match image with
            | null -> String.Empty
            | _ -> 
                let url = image.Element(xn "url")
                match url with
                | null -> String.Empty
                | _ ->
                    if url.Value = null || url.Value = String.Empty then
                        String.Empty
                    else
                        url.Value

    let private getImageForItem (item : XElement) : string =
        let imageElement = getElementUsingLocalNameAndNamespace item "image" "http://www.itunes.com/dtds/podcast-1.0.dtd"
        match imageElement with
        | null -> String.Empty
        | _ -> getValueFromAttribute imageElement "href"

    let private getFileSizeFromElement (element : XElement) (attributeName : string) =
    
        let length = getValueFromAttribute element attributeName
        match length with
        | null | "" -> -1L
        | _ -> length |> Int64.Parse

    let private isAudioFile mimeType : bool =
        mimeType = "audio/mpeg3" || mimeType = "audio/mpeg"

    let private getAllFileDetailsForEnclosureElements (item : XElement) =
        [for enclosureElement in item.Descendants(xn "enclosure") do
            if getValueFromAttribute enclosureElement "type" |> isAudioFile then
                yield {
                    FileSize = getFileSizeFromElement enclosureElement "length"
                    DownloadDate = NoDateTime
                    ImageFileName = ""
                }
        ]

    let private getAllFileDetailsForMediaContentElements (item : XElement) () = 
        [for contentElement in item.Descendants(XName.Get("content", "http://search.yahoo.com/mrss/")) do
            if getValueFromAttribute contentElement "type" |> isAudioFile then
                yield {
                    FileSize = getFileSizeFromElement contentElement "fileSize"
                    DownloadDate = NoDateTime
                    ImageFileName = ""
                }
        ]

    let private createPodcastArrayFromDocument (document: XDocument) =

        [for item in document.Descendants(xn "item") do
            let titleElement = item?title
            let title = getTitleForItem titleElement

            let enclosureElement = item?enclosure
            let contentElement = getElementUsingLocalNameAndNamespace item "content" "http://search.yahoo.com/mrss/"

            let allFileDetailsForEnclosureElements = getAllFileDetailsForEnclosureElements item

            let allFileDetailsForMediaContentElements = getAllFileDetailsForMediaContentElements item
            
            let url = getURLForItem enclosureElement contentElement

            if title <> String.Empty && url <> String.Empty then
                yield {
                    Title = title
                    Description = getDescriptionFromItem item
                    PubDate = getPubDateFromItem item
                    URL = url
                    ImageURL = getImageForItem item
                    FileDetails = 
                    {
                        FileSize = getFilesizeForItem enclosureElement contentElement
                        DownloadDate = NoDateTime
                        ImageFileName = ""
                    }
                }
          ] |> List.toArray
            
    let private mergeFeeds (oldFeed : Feed) (newFeed : Feed) : Feed =

        let mutable newIndex = 0
        let mutable oldIndex = 0
        while newIndex < newFeed.Podcasts.Length && 
          oldIndex < oldFeed.Podcasts.Length &&
          oldFeed.Podcasts.[oldIndex] <> newFeed.Podcasts.[newIndex] do
            newIndex <- newIndex + 1

        while oldIndex < oldFeed.Podcasts.Length && newIndex < newFeed.Podcasts.Length do
            let oldPodcast = oldFeed.Podcasts.[oldIndex]
            let newPodcast = newFeed.Podcasts.[newIndex]
            if oldPodcast = newPodcast then
                let fileDetails = oldPodcast.FileDetails
                newPodcast.SetAllFileDetails fileDetails.FileSize fileDetails.DownloadDate fileDetails.ImageFileName
            oldIndex <- oldIndex + 1
            newIndex <- newIndex + 1
        newFeed

    let private createFeedRecord url directoryPath imageFileName creationDate (document : XDocument) : Feed =
        let channel = document.Element(xn "rss").Element(xn "channel")
        let imageFileURL = getImageForChannel channel

        {
             Title = channel?title.Value
             Description = CleanText channel?description.Value
             Website = channel?link.Value
             Directory = directoryPath
             URL = url
             ImageURL = imageFileURL
             ImageFileName = imageFileName
             Podcasts = createPodcastArrayFromDocument document
             CreationDateTime = creationDate
             UpdatedDateTime = NoDateTime
        }

    let private resolveImages (imageResolver : IImageResolver) (cancelToken : CancellationToken) (feed : Feed) : Feed =

      match (imageResolver) with
      | null -> feed
      | _ ->
        let mutable index = 0
        while index < feed.Podcasts.Length do
          let podcast = feed.Podcasts.[index]

          let localImagePath = imageResolver.GetName podcast.FileDetails.ImageFileName podcast.ImageURL
          if localImagePath <> podcast.FileDetails.ImageFileName then
            podcast.SetImageFileName localImagePath
        
          cancelToken.ThrowIfCancellationRequested()
          
          index <- index + 1

        let localImagePath = imageResolver.GetName feed.ImageFileName feed.ImageURL  
        if (localImagePath <> feed.ImageFileName) then
            Feed.SetImageFileName feed localImagePath
        else
            feed
    
    let private createWebClient : WebClient = 
        let webClient = new WebClient()
        webClient.Headers.Add("user-agent", "Podful Podcatcher")
        webClient.Encoding <- System.Text.Encoding.UTF8;
        webClient
       
    let private createXMLDocumentFromData data : XDocument =
        XDocument.Parse(data)
        
    let private downloadFeedData url retryCount (webClient : WebClient) : string = 
        let uri = Uri(url)

        let mutable data = null
        let mutable tryCount = retryCount
        while tryCount > 0 do
            try
                data <- webClient.DownloadString(uri)
                tryCount <- 0
            with
            | _ as ex ->
                    tryCount <- tryCount - 1
                    if retryCount = 0 then
                        reraise()

        data

    let private saveToFile filePath (data : string) : string =
        use writer = new StreamWriter(filePath, false)
        writer.Write(data)
        data

    let public CreateFeed url directoryPath imageResolver cancelToken = 
        createWebClient |>
        downloadFeedData url 5 |>
        createXMLDocumentFromData |>
        createFeedRecord url directoryPath String.Empty DateTime.Now |>
        resolveImages imageResolver cancelToken
       
    let public CreateFeedFromFile url filePath directoryPath imageResolver cancelToken : Feed =
        createWebClient |>
        downloadFeedData url 5 |>
        saveToFile filePath |>
        createXMLDocumentFromData |>
        createFeedRecord url directoryPath String.Empty DateTime.Now |>
        resolveImages imageResolver cancelToken

    let public UpdateFeed feed imageResolver cancelToken : Feed = 
        createWebClient |>
        downloadFeedData feed.URL 5 |>
        createXMLDocumentFromData |>
        createFeedRecord feed.URL feed.Directory feed.ImageFileName feed.CreationDateTime |> 
        Feed.SetUpdatedDate feed.UpdatedDateTime |> 
        mergeFeeds feed |>
        resolveImages imageResolver cancelToken

    let public UpdateFeedFromFile feed filePath imageResolver cancelToken : Feed = 
        createWebClient |>
        downloadFeedData feed.URL 5 |>
        saveToFile filePath |>
        createXMLDocumentFromData |>
        createFeedRecord feed.URL feed.Directory feed.ImageFileName feed.CreationDateTime |> 
        Feed.SetUpdatedDate feed.UpdatedDateTime |> 
        mergeFeeds feed |>
        resolveImages imageResolver cancelToken
