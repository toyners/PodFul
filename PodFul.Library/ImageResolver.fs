namespace PodFul.Library

open System
open System.IO
open System.Collections.Generic
open Jabberwocky.Toolkit.String

type ImageResolverOld(imageDirectoryPath : string, defaultImagePath : string, returnDefaultImageOnException : Boolean) =

    let directoryPath = imageDirectoryPath
    let defaultImagePath = defaultImagePath
    let returnDefaultImageOnException = returnDefaultImageOnException
    let mutable renameFunction = Miscellaneous.NextImageFileName
    let mutable postMessage : Action<string> = null

    member private this.tryPostMessage message = 
        if Object.ReferenceEquals(postMessage, null) <> true then 
            postMessage.Invoke(message)

    member private this.downloadImageFile urlPath savePath =
        let imageDownloader = new FileDownloader()
        let mutable localPath = savePath

        let message = "Downloading '" + urlPath + "' ... "
        this.tryPostMessage message

        try
            imageDownloader.Download(urlPath, savePath, System.Threading.CancellationToken.None, null) |> ignore

            this.tryPostMessage "Complete\r\n"
        with
        | _ -> 
            if returnDefaultImageOnException = true then
                localPath <- defaultImagePath
                this.tryPostMessage "FAILED\r\n"
            else
                reraise()

        localPath

    member public this.RenameFunction
        with set(v : string -> string) =
            renameFunction <- v

    interface IImageResolverOld with

        member this.PostMessage 
            with get() : Action<string> = 
                postMessage    
            and set(v : Action<string>) =
                postMessage <- v

        member this.GetName (localPath : string) (urlPath : string) : string = 

            let gotLocalPath = String.IsNullOrEmpty(localPath) = false
            let gotURLPath = String.IsNullOrEmpty(urlPath) = false

            if (gotLocalPath = false || localPath = defaultImagePath) then
                
                if gotURLPath then    
                    let localName = renameFunction urlPath
                    let savePath = Path.Combine(directoryPath, localName)
                    this.downloadImageFile urlPath savePath
                else
                    defaultImagePath
                
            else if gotLocalPath then
                 if (File.Exists(localPath)) = false then
                    this.downloadImageFile urlPath localPath |> ignore
                 localPath    
            else
                failwith "Not Implemented"
            
    