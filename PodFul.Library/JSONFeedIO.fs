namespace PodFul.Library

open System.IO
open System.Runtime.Serialization.Json
open System.Text

module JSONFeedIO =

  let ResolveFeedDifferences (oldFeed : OldFeed) : Feed = 
    {
        Title = oldFeed.Title
        Description = oldFeed.Description
        Website = oldFeed.Website
        Directory = oldFeed.Directory
        URL = oldFeed.URL
        ImageURL = oldFeed.ImageURL
        ImageFileName = oldFeed.ImageFileName
        Podcasts = oldFeed.Podcasts
        CreationDateTime = oldFeed.CreationDateTime
        UpdatedDateTime = oldFeed.UpdatedDateTime
        DoScan = true
        CompleteDownloadsOnScan = true
        DeliverDownloadsOnScan = true
    }

  let ReadFeedFromFile (filePath : string) : Feed =
    use reader = new StreamReader(filePath) 
    use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(reader.ReadToEnd())) 

    (new DataContractJsonSerializer(typeof<Feed>)).ReadObject(ms) :?> Feed
    //    let obj = (new DataContractJsonSerializer(typeof<OldFeed>)).ReadObject(ms) 
      //  obj :?> OldFeed |> ResolveFeedDifferences

  let WriteFeedToFile (feed : Feed) (filePath : string) : unit = 
    use ms = new MemoryStream()
    (new DataContractJsonSerializer(typeof<Feed>)).WriteObject(ms, feed)
    
    use writer = new StreamWriter(filePath)
    writer.Write(Encoding.Default.GetString(ms.ToArray()))

  