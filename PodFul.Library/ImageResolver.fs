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

    interface IImageResolver with

        member this.PostMessage 
            with get() : Action<string> = 
                postMessage    
            and set(v : Action<string>) =
                postMessage <- v

        member this.GetName (imageFileName : string) : string = 

            match String.IsNullOrEmpty(imageFileName) with
            | true -> defaultImagePath
            | _ ->
                match File.Exists(imageFileName) with
                | true -> imageFileName
                | _ ->
                    let cleanImageFileName = imageFileName.Substitute(this.fileNameSubstitutions);
                    let mutable imageFilePath = Path.Combine(directoryPath, cleanImageFileName)

                    if Object.ReferenceEquals(postMessage, null) <> true then 
                        let message = "Downloading " + imageFileName + " ...\r\n"
                        postMessage.Invoke(message)

                    let imageDownloader = new FileDownloader()

                    try
                        imageDownloader.Download(imageFileName, imageFilePath, System.Threading.CancellationToken.None, null) |> ignore
                    with
                    | _ -> 
                        if returnDefaultImageOnException = true then
                            imageFilePath <- defaultImagePath
                        else
                            reraise()

                    imageFilePath

        member this.GetName2 (localPath : string) (urlPath : string) : string = 

            let gotLocalPath = String.IsNullOrEmpty(localPath) = false
            let gotURLPath = String.IsNullOrEmpty(urlPath) = false

            if (gotLocalPath = false || localPath = defaultImagePath) then
                
                if gotURLPath then    
                    let localName = renameFunction urlPath
                    let savePath = Path.Combine(directoryPath, localName)
                    let fileDownloader = new FileDownloader()
                    fileDownloader.Download(urlPath, savePath, System.Threading.CancellationToken.None, null) |> ignore
                    savePath
                else
                    defaultImagePath
                
            else if gotLocalPath then
                 if (File.Exists(localPath)) = false then
                    let fileDownloader = new FileDownloader()
                    fileDownloader.Download(urlPath, localPath, System.Threading.CancellationToken.None, null) |> ignore
                 localPath    
            else
                failwith "Not Implemented"
            
            