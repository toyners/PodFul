namespace PodFul.Library

open System
open System.IO

type LocationDeliveryPoint(directoryPath, postMessage : Action<string>) =

    let directoryPath = directoryPath
    let postMessage = postMessage

    member this.DeliverToLocation (podcast : Podcast) (filePath : string) : unit = 
        
        try
            let destinationPath = Miscellaneous.SwapDirectoryInFilePath directoryPath filePath
            File.Copy(filePath, destinationPath)
            postMessage.Invoke("Copied \"" + podcast.Title + "\" to \"" + directoryPath + "\"")
        with ex ->
            postMessage.Invoke("Failed to copy \"" + podcast.Title + "\" to \"" + directoryPath + "\": " + ex.Message);
