namespace PodFul.Library

open System
open System.IO

type LocationDeliveryPoint(directoryPath, postMessage : Action<string>) =

    let directoryPath = directoryPath
    let postMessage = postMessage

    member this.DeliverToLocation(filePath : string) : bool = 
        try
            let destinationPath = Miscellaneous.SwapDirectoryInFilePath directoryPath filePath
            File.Copy(filePath, destinationPath)
            postMessage.Invoke("")
            true
        with ex ->
            postMessage.Invoke(ex.Message); false
