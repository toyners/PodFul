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
            element.Value

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

    let private createPodcastArrayFromDocument (document: XDocument) =

        [for element in document.Descendants(xn "item") do
            let titleElement = element?title
            let enclosureElement = element?enclosure
            let contentElement = getElementUsingLocalNameAndNamespace element "content" "http://search.yahoo.com/mrss/"

            let title = getTitleForItem titleElement
            let url = getURLForItem enclosureElement contentElement

            if title <> String.Empty && url <> String.Empty then
                yield {
                    Title = title
                    Description = getDescriptionFromItem element
                    PubDate = getPubDateFromItem element
                    URL = url
                    FileSize = getFilesizeForItem enclosureElement contentElement
                    DownloadDate = NoDateTime
                    ImageFileName = getImageForItem element
                }
          ] |> List.toArray

    exception RetryException of Exception

    let rec tryDownloadDocument (webClient : WebClient) (uri : Uri) retryCount : XDocument =
        try
            let data = webClient.DownloadString(uri)
            XDocument.Parse(data)
        with
        | _ as ex ->
            if retryCount > 0 then
                tryDownloadDocument webClient uri (retryCount - 1)
            else
                raise (RetryException ex)

    let private downloadDocument(url) : XDocument =

        try
            let webClient = new WebClient()
            webClient.Headers.Add("user-agent", "Podful Podcatcher")
            webClient.Encoding <- System.Text.Encoding.UTF8;

            let uri = Uri(url)
        
            tryDownloadDocument webClient uri 5
            
        with
        | :? RetryException as retryex ->
            if retryex.InnerException = null then
                reraise()
                
            match (retryex.InnerException :? System.Net.WebException) with
            | false -> 
                reraise()
            | _ ->
                let webex = retryex.InnerException :?> System.Net.WebException
                match webex.Response with
                | null -> reraise()
                | _ -> 
                    use streamReader = new StreamReader(webex.Response.GetResponseStream())
                    let errorLogFilePath = Path.GetTempPath() + "PodFul.log"
                    use streamWriter = new StreamWriter(errorLogFilePath)
                    let responseText = streamReader.ReadToEnd()
                    streamWriter.Write(responseText)
                    let message = webex.Message + ". Error log written to '" + errorLogFilePath + "'."
                    raise (RetryException (new System.Net.WebException(message)))
            
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
                newFeed.Podcasts.[newIndex] <- Podcast.SetDownloadDate oldPodcast.DownloadDate newPodcast |> 
                    Podcast.SetFileSize oldPodcast.FileSize
            oldIndex <- oldIndex + 1
            newIndex <- newIndex + 1
        newFeed

    let private createFeedRecord url directoryPath creationDate (document : XDocument) : Feed =
        let channel = document.Element(xn "rss").Element(xn "channel")
        let imageFileURL = getImageForChannel channel

        {
             Title = channel?title.Value
             Description = CleanText channel?description.Value
             Website = channel?link.Value
             Directory = directoryPath
             URL = url
             ImageFileName = imageFileURL
             Podcasts = createPodcastArrayFromDocument document
             CreationDateTime = creationDate
             UpdatedDateTime = NoDateTime
        }

    let private resolveImages (imageResolver : IImageResolver) (feed : Feed) : Feed =

      match (imageResolver) with
      | null -> feed
      | _ ->
        let mutable index = 0
        while index < feed.Podcasts.Length do
          let podcast = feed.Podcasts.[index]
          let imageFileName = imageResolver.GetName podcast.ImageFileName
          if (imageFileName <> podcast.ImageFileName) then
            feed.Podcasts.[index] <- Podcast.SetImageFileName imageFileName podcast
        
          index <- index + 1

        let imageFileName = imageResolver.GetName feed.ImageFileName  
        match (imageFileName <> feed.ImageFileName) with
        | false -> feed
        | _ -> Feed.SetImageFileName feed imageFileName

    let public CreateFeed url directoryPath imageResolver = 
        downloadDocument url |> 
        createFeedRecord url directoryPath DateTime.Now |>
        resolveImages imageResolver

    let public UpdateFeed (feed : Feed) (imageResolver : IImageResolver) : Feed = 
        downloadDocument feed.URL |> 
        createFeedRecord feed.URL feed.Directory feed.CreationDateTime |>  
        Feed.SetUpdatedDate feed.UpdatedDateTime |> 
        mergeFeeds feed |>
        resolveImages imageResolver
