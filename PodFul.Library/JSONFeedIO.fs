namespace PodFul.Library

open System.IO
open System.Runtime.Serialization.Json
open System.Text

module JSONFeedIO =

  let canResolveDifferences (exceptionMessage : string) : bool =
    exceptionMessage.Contains("The data contract type 'PodFul.Library.Feed' cannot be deserialized because the required data member 'ConfirmDownloadThreshold@' was not found.")
    
  let ResolveFeedDifferences (previousFeed : PreviousFeed) : Feed = 
    {
        Title = previousFeed.Title
        Description = previousFeed.Description
        Website = previousFeed.Website
        Directory = previousFeed.Directory
        URL = previousFeed.URL
        ImageURL = previousFeed.ImageURL
        ImageFileName = previousFeed.ImageFileName
        Podcasts = previousFeed.Podcasts
        CreationDateTime = previousFeed.CreationDateTime
        UpdatedDateTime = previousFeed.UpdatedDateTime
        DoScan = previousFeed.DoScan
        CompleteDownloadsOnScan = previousFeed.CompleteDownloadsOnScan
        DeliverDownloadsOnScan = previousFeed.DeliverDownloadsOnScan
        ConfirmDownloadThreshold = 3 // Set to default
    }

  let ReadFeedFromFile (filePath : string) : Feed =
    use reader = new StreamReader(filePath) 
    use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(reader.ReadToEnd())) 

    try
        (new DataContractJsonSerializer(typeof<Feed>)).ReadObject(ms) :?> Feed
    with
    | :? System.Runtime.Serialization.SerializationException as e ->
        if not (canResolveDifferences e.Message) then
            reraise()

        // Reset to the start of the data.
        ms.Position <- 0L

        // Resolve the data into an instance of PreviousFeed before casting it to the Feed instance
        // by resolving the differences.
        let obj = (new DataContractJsonSerializer(typeof<PreviousFeed>)).ReadObject(ms)
        obj :?> PreviousFeed |> ResolveFeedDifferences

  let WriteFeedToFile (feed : Feed) (filePath : string) : unit = 
    use ms = new MemoryStream()
    (new DataContractJsonSerializer(typeof<Feed>)).WriteObject(ms, feed)
    
    use writer = new StreamWriter(filePath)
    writer.Write(Encoding.Default.GetString(ms.ToArray()))

  