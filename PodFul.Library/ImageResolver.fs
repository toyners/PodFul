namespace PodFul.Library

open System
open System.IO
open System.Collections.Generic
open Jabberwocky.Toolkit.String

type ImageResolver(imageDirectoryPath : string, defaultImagePath : string, returnDefaultImageOnException : Boolean, renameFunction : string -> string) =

    let directoryPath = imageDirectoryPath
    let defaultImagePath = defaultImagePath
    let returnDefaultImageOnException = returnDefaultImageOnException
    let renameFunction = renameFunction
    let mutable postMessage : Action<string> = null

    member private this.fileNameSubstitutions = Dictionary<String, String>(dict
                                                    [
                                                        ( "\\", "_bs_" );
                                                        ( "/", "_fs_" );
                                                        ( ":", "_c_" );
                                                        ( "*", "_a_" );
                                                        ( "?", "_q_" );
                                                        ( "\"", "_qu_" );
                                                        ( "<", "_l_" );
                                                        ( ">", "_g_" );
                                                        ( "|", "_b_" )
                                                    ])
    member private this.downloadImageFile urlPath savePath =
        let imageDownloader = new FileDownloader()
        let mutable localPath = savePath

        try
            imageDownloader.Download(urlPath, savePath, System.Threading.CancellationToken.None, null) |> ignore
        with
        | _ -> 
            if returnDefaultImageOnException = true then
                localPath <- defaultImagePath
            else
                reraise()

        localPath

    interface IImageResolver with

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
            
    