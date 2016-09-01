namespace PodFul.Library

open System
open System.Collections.Generic
open System.IO
open System.Linq
open Jabberwocky.Toolkit.String

type NativeFeedIO() =
  
  static member private readLineFromFile (reader: StreamReader) : string = 
    let text = reader.ReadLine()
    match text with
    | null ->   failwith "Raw text is null."
    | "" ->     failwith "Raw text is empty."
    | _ -> text

  static member private splitStringUsingDelimiter (text : string) : string[] = text.Split('|')

  static member private verifyFields (fields: string[]) : string[] =
    if fields = null then
      failwith "Fields array is null."
    else if fields.Length = 0 then
      failwith "Fields array is empty."
    else if fields.Length < 7 then
      failwith ("Fields array only has " + fields.Length.ToString() + " field(s).")
    else
      fields

  static member private getImageFileName (fields : string[]) : string =
    if fields.Length > 5 then
      fields.[5]
    else
      String.Empty

  static member private getDateTimeAt (fields : string[]) index : DateTime =
    if fields.Length > index then
      let dateTime = DateTime.Parse(fields.[index])
      match (dateTime = DateTime.MinValue) with
      | true -> FeedFunctions.NoDateTime
      | _ -> dateTime
    else
      FeedFunctions.NoDateTime

  static member private getPodcastFromFile (reader: StreamReader) = 
    match reader.EndOfStream with
    | true -> None
    | _ ->
      // Create fields array using line read from reader.
      let fields = NativeFeedIO.readLineFromFile reader |> 
                   NativeFeedIO.splitStringUsingDelimiter |> 
                   NativeFeedIO.verifyFields

      // Create the podcast record.
      let podcast = 
        {
          Title = fields.[0]
          PubDate = DateTime.Parse(fields.[1])
          URL = fields.[2]
          Description = fields.[4]
          FileDetails =
          {
              FileSize = Int64.Parse(fields.[3])
              DownloadDate = NativeFeedIO.getDateTimeAt fields 5
              ImageFileName = fields.[6]
          }
        }

      // Set the threaded state to be the XML reader.
      Some(podcast, reader)

  static member public ReadFeedFromFile (filePath : String) : Feed =
        
    use reader = new StreamReader(filePath)
    let fields =  NativeFeedIO.readLineFromFile reader |> NativeFeedIO.splitStringUsingDelimiter

    { 
      Title = fields.[0]
      Website = fields.[1]
      Directory = fields.[2]
      URL = fields.[3]
      Description = fields.[4]
      ImageFileName = NativeFeedIO.getImageFileName fields
      CreationDateTime = NativeFeedIO.getDateTimeAt fields 6
      UpdatedDateTime = NativeFeedIO.getDateTimeAt fields 7
      Podcasts = List.unfold NativeFeedIO.getPodcastFromFile (reader) |> List.toArray
    }

  static member public WriteFeedToFile (feed : Feed) (filePath : string) : unit =
        
    use writer = new StreamWriter(filePath)

    writer.WriteLine(feed.Title + "|" + feed.Website + "|" + feed.Directory + "|" + feed.URL + "|" + feed.Description + "|" + feed.ImageFileName + "|" + feed.CreationDateTime.ToString() + "|" + feed.UpdatedDateTime.ToString() + "|");

    for podcast in feed.Podcasts do
      writer.WriteLine(podcast.Title + "|" + 
        podcast.PubDate.ToString() + "|" + 
        podcast.URL + "|" + 
        podcast.FileDetails.FileSize.ToString() + "|" +
        podcast.Description + "|" +
        podcast.FileDetails.DownloadDate.ToString() + "|" +
        podcast.FileDetails.ImageFileName + "|")

