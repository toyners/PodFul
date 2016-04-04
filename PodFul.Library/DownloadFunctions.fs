namespace PodFul.Library

open System
open System.Net
open System.Xml.Linq

module DownloadFunctions =

    let xn name = XName.Get(name)

    // This implementation of the dynamic operator ? returns the child element from the parent that matches the name.
    let (?) (parent : XElement) name = 
        let child = parent.Element(xn name)
        match child with
        | null -> failwith ("Element '" + name + "' not found in '" + parent.Name.LocalName + "'")
        | _ -> child;

    let GetAttributeValue (element: XElement) name = 
        let attribute = element.Attribute(xn name)
        match attribute with 
        | null -> failwith ("Atributr '" + name + "' not found in '" + element.Name.LocalName + "'")
        | _ -> attribute.Value

    let DownloadRSSFeed(url) = 
        let webClient = new WebClient()
        let data = webClient.DownloadString(Uri(url))
        let document = XDocument.Parse(data)
        let channel = document.Element(xn "rss").Element(xn "channel")

        {
             Title = channel?title.Value
             Description = channel?description.Value
             Website = channel?link.Value
             Directory = null
             Feed = null
             Podcasts = [ for element in document.Descendants(xn "item") do
                            yield {
                                Title = element?title.Value
                                Description = element?description.Value
                                PubDate = element?pubDate.Value |> DateTime.Parse
                                URL = GetAttributeValue element?enclosure "url"
                                FileSize = GetAttributeValue element?enclosure "length" |> Int64.Parse
                            }] |> List.toArray
        }

