namespace PodFul.Library

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Runtime.Serialization.Json
open System.Text
open Jabberwocky.Toolkit.String

module JSONFeedIO =

  let ReadFeedFromFile (filePath : string) : Feed =
    use reader = new StreamReader(filePath) 
    use ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(reader.ReadToEnd())) 
    let obj = (new DataContractJsonSerializer(typeof<Feed>)).ReadObject(ms) 
    obj :?> Feed

  let WriteFeedToFile (feed : Feed) (filePath : string) : unit = 
    use ms = new MemoryStream()
    (new DataContractJsonSerializer(typeof<Feed>)).WriteObject(ms, feed)
    
    use writer = new StreamWriter(filePath)
    writer.Write(Encoding.Default.GetString(ms.ToArray()))