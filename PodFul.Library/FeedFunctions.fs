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
        let mutable length = -1
        while length <> cleanText.Length do
          length <- cleanText.Length
          cleanText <- cleanText.Replace("  ", " ")

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

    let private getTitleForItem (item : XElement) : string =
        let titleElement = item?title
        if titleElement = null || titleElement.Value = null || titleElement.Value = String.Empty then
            String.Empty
        else
            titleElement.Value.Replace("\r", String.Empty).Replace("\n", String.Empty).Replace("\t", String.Empty)

    let private getImageForEntity (element : XElement) : string =
        let imageElement = getElementUsingLocalNameAndNamespace element "image" "http://www.itunes.com/dtds/podcast-1.0.dtd"
        match imageElement with
        | null -> String.Empty
        | _ -> getValueFromAttribute imageElement "href"

    let private getImageForChannel (channel : XElement) : string =

        let getImageFromURLTag (element : XElement) : string =
            let image = channel.Element(xn "image")
            match image with
            | null -> String.Empty
            | _ -> 
                let url = image.Element(xn "url")
                match url with
                | null -> String.Empty
                | _ ->
                    match url.Value with
                    | null | "" -> String.Empty
                    | _ -> url.Value

        match channel with
            | null -> String.Empty
            | _ ->
                let imageValue = getImageFromURLTag channel
                match imageValue with
                | null | "" -> getImageForEntity channel
                | _ -> imageValue

    let private getFileSizeFromElement (element : XElement) (attributeName : string) =
    
        try
          let length = getValueFromAttribute element attributeName
          match length with
          | null | "" -> -1L
          | _ -> Int64.Parse(length, Globalization.NumberStyles.Number)
        with
        | _ -> -1L

    let private isAudioFile mimeType : bool =
        mimeType = "audio/mpeg3" || mimeType = "audio/mpeg" || mimeType = "audio/mp3"

    let private enclourseElementName : XName = xn "enclosure"

    let private mediaContentElementName : XName = XName.Get("content", "http://search.yahoo.com/mrss/")

    let private getFileDetailsForPodcasts (item : XElement) fileSizeAttributeName nameFunc : List<string * Int64> =
        
        [for enclosureElement in item.Descendants(nameFunc) do
            if getValueFromAttribute enclosureElement "type" |> isAudioFile then
                let url = getValueFromAttribute enclosureElement "url"
                let fileSize = getFileSizeFromElement enclosureElement fileSizeAttributeName
                yield url,fileSize
        ]

    let private getAllFileDetailsForPodcasts item : List<string * Int64> =
        List.append (getFileDetailsForPodcasts item "length" enclourseElementName) 
                    (getFileDetailsForPodcasts item "fileSize" mediaContentElementName)

    let private fileDetailsListWithoutDuplicates (list : List<string * Int64>) : List<string * Int64> =
        let mutable urls = Set.empty
        [for file in list do
            if not <| Set.contains (fst file) urls then
                urls <- Set.add (fst file) urls
                yield file
        ]

    let private getPodcastTitle baseTitle fileDetailsNumber fileDetailsCount = 
        if fileDetailsCount = 1 then
            baseTitle
        else
            baseTitle + " [" + fileDetailsNumber.ToString() + " of " + fileDetailsCount.ToString() + "]"

    let private finaliseFileNames (podcasts : list<Podcast>) : list<Podcast> =
        podcasts 

    let private createPodcastArrayFromDocument (document: XDocument) : Podcast[] =

        try
          let items = document.Descendants(xn "item")
          [for item in items do
            
              let baseTitle = getTitleForItem item
              let imageURL = getImageForEntity item
              let description = getDescriptionFromItem item
              let pubDate = getPubDateFromItem item

              if baseTitle <> String.Empty then
                  let fileDetails = getAllFileDetailsForPodcasts item |> fileDetailsListWithoutDuplicates
                  let fileDetailsCount = List.length fileDetails
                  let mutable fileDetailsNumber = 1
                  for fileDetail in fileDetails do
                      let url = fst fileDetail
                    
                      if url <> String.Empty then
                          let title = getPodcastTitle baseTitle fileDetailsNumber fileDetailsCount
                          fileDetailsNumber <- fileDetailsNumber + 1

                          let podcast = {
                              Title = title
                              Description = description
                              PubDate = pubDate
                              URL = url
                              ImageURL = imageURL
                              FileDetails =
                              {
                                  FileSize = snd fileDetail
                                  DownloadDate = NoDateTime
                                  ImageFileName = ""
                                  FileName = ""
                              }
                          }

                          yield podcast
            ] |> finaliseFileNames |> List.toArray
        with
        | _ as ex -> failwith ("error: " + ex.Message)          
            
    let private updateFileDetailsOnPodcastsForNewFeed (oldFeed : Feed) (newFeed : Feed) : Feed =

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

    let private createFeedRecord url directoryPath feedImageFilePath creationDate (document : XDocument) : Feed =
        let channel = document.Element(xn "rss").Element(xn "channel")
        let imageFileURL = getImageForChannel channel

        {
             Title = channel?title.Value
             Description = CleanText channel?description.Value
             Website = channel?link.Value
             Directory = directoryPath
             URL = url
             ImageURL = imageFileURL
             ImageFileName = feedImageFilePath
             Podcasts = createPodcastArrayFromDocument document
             CreationDateTime = creationDate
             UpdatedDateTime = DateTime.Now
             DoScan = true
             CompleteDownloadsOnScan = true
             DeliverDownloadsOnScan = true
        }

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
                    if tryCount = 0 then
                        reraise()

        data

    let private saveToFile (filePath : string) (data : string) : string =

        match (String.IsNullOrEmpty(filePath)) with
        | false -> 
            use writer = new StreamWriter(filePath, false)
            writer.Write(data)
            data
        | _ -> 
            data

    let private setAllPodcastsToHaveDefaultImage (defaultImagePath : string) (feed : Feed) : Feed = 
        for podcast in feed.Podcasts do
            podcast.SetImageFileName defaultImagePath    
        feed

    let private setFileNamesForPodcasts (feed : Feed) : Feed =
        FinalisingFileNameFunctions.finaliseFileNames feed.Title feed.Podcasts |> ignore
        feed

    let public CreateFeed url saveFilePath directoryPath defaultImagePath cancelToken : Feed =
        createWebClient |>
        downloadFeedData url 5 |>
        saveToFile saveFilePath |>
        createXMLDocumentFromData |>
        createFeedRecord url directoryPath String.Empty DateTime.Now |>
        setAllPodcastsToHaveDefaultImage defaultImagePath |>
        setFileNamesForPodcasts

    let public UpdateFeed feed filePath cancelToken : Feed = 
        createWebClient |>
        downloadFeedData feed.URL 5 |>
        saveToFile filePath |>
        createXMLDocumentFromData |>
        createFeedRecord feed.URL feed.Directory feed.ImageFileName feed.CreationDateTime |> 
        Feed.SetUpdatedDate feed.UpdatedDateTime |>
        Feed.SetScanningFlags feed.DoScan feed.CompleteDownloadsOnScan feed.DeliverDownloadsOnScan |> 
        updateFileDetailsOnPodcastsForNewFeed feed |>
        setFileNamesForPodcasts

