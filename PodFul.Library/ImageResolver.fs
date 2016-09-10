namespace PodFul.Library

open System
open System.IO
open System.Collections.Generic
open Jabberwocky.Toolkit.String

type ImageResolver(imageDirectoryPath : string, defaultImagePath : string) =

    let directoryPath = imageDirectoryPath
    let defaultImagePath = defaultImagePath
    let mutable postMessage : Action<string> = null
    let mutable returnDefaultImageOnException = false

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
        
        // Gets or sets a value indicating whether the default image name from GetName if 
        // an exception is thrown during download.
        member this.ReturnDefaultImageOnException
            with get() : Boolean = 
                returnDefaultImageOnException
            and set(v : Boolean) =
                returnDefaultImageOnException <- v

        member this.PostMessage 
            with get() : Action<string> = 
                postMessage    
            and set(v : Action<string>) =
                postMessage <- v

        member this.GetName (imageFileName : string) : string = 

            match String.IsNullOrEmpty(imageFileName) with
            | true -> defaultImagePath
            | _ ->
                let cleanImageFileName = imageFileName.Substitute(this.fileNameSubstitutions);
                let mutable imageFilePath = Path.Combine(directoryPath, cleanImageFileName)

                if File.Exists(imageFilePath) = false then
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