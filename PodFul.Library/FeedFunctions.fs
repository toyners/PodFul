﻿namespace PodFul.Library

open System
open System.IO
open System.Net
open System.Threading
open System.Xml.Linq
open FSharp.Data

module public FeedFunctions =

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
            DateTime.MinValue
        else
            pubDateElement.Value |> removeTimeZoneAbbreviationsFromDateTimeString |> DateTime.Parse

    let public CleanText (text: string) : string = 
        // Replace line breaks and multiple spaces with single spaces
        let mutable fixedText = text.Replace("\r\n", " ").Replace("\n", " ")
        while fixedText.IndexOf("  ") <> -1 do 
            fixedText <- fixedText.Replace("  ", " ")

        // Replace HTML code for right single quotation mark
        fixedText <- fixedText.Replace("<p>", String.Empty)
            .Replace("</p>", String.Empty)
            .Replace("<P>", String.Empty)
            .Replace("</P>", String.Empty)
            .Replace("&#8217;", "'")
            .Replace("&#124;", "")
            .Replace("&#8230;", "...") // Actually should be the ellipsis character but I'm going to use three dots instead.

        fixedText

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
                    DownloadDate = DateTime.MinValue
                    ImageFileName = getImageForItem element
                }
          ] |> List.toArray

    let private downloadDocument(url) : XDocument = 
        try
            let webClient = new WebClient()
            webClient.Headers.Add("user-agent", "Podful Podcatcher")
            let data = webClient.DownloadString(Uri(url))
            XDocument.Parse(data)
        with
        | :? System.Net.WebException as webex ->
             match webex.Response with
             | null -> failwith webex.Message
             | _ ->
                 use streamReader = new StreamReader(webex.Response.GetResponseStream())
                 let errorLogFilePath = Path.GetTempPath() + "PodFul.log"
                 use streamWriter = new StreamWriter(errorLogFilePath)
                 let responseText = streamReader.ReadToEnd()
                 streamWriter.Write(responseText)
                 failwith ("Error log written to '" + errorLogFilePath + "'.")

    let private mergeFeeds (oldFeed : Feed) (newFeed : Feed) : Feed =

        let mutable newIndex = 0
        let mutable oldIndex = 0
        while newIndex < newFeed.Podcasts.Length && oldFeed.Podcasts.[oldIndex] <> newFeed.Podcasts.[newIndex] do
            newIndex <- newIndex + 1

        while oldIndex < oldFeed.Podcasts.Length && newIndex < newFeed.Podcasts.Length do
            let oldPodcast = oldFeed.Podcasts.[oldIndex]
            let newPodcast = newFeed.Podcasts.[newIndex]
            if oldPodcast = newPodcast then
                newFeed.Podcasts.[newIndex] <- Podcast.SetDownloadDate oldPodcast.DownloadDate newPodcast |> 
                    Podcast.SetFileSize oldPodcast.FileSize |>
                    Podcast.SetImageFileName oldPodcast.ImageFileName 
            oldIndex <- oldIndex + 1
            newIndex <- newIndex + 1
        newFeed

    let private createFeedRecord url directoryPath (document : XDocument) : Feed =
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
             CreationDateTime = DateTime.Now
        }

    let public CreateFeed url directoryPath =
        downloadDocument url |> createFeedRecord url directoryPath

    let public UpdateFeed (feed : Feed) : Feed = 
        downloadDocument feed.URL |> createFeedRecord feed.URL feed.Directory |> mergeFeeds feed 