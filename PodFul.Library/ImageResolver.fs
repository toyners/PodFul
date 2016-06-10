namespace PodFul.Library

open System
open System.IO
open System.Collections.Generic
open Jabberwocky.Toolkit.String

type ImageResolver(imageDirectoryPath : string) =

    let directoryPath = imageDirectoryPath

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
        
        member this.GetName (imageFileName : string) : string = 

            match String.IsNullOrEmpty(imageFileName) with
            | true -> String.Empty
            | _ ->
                let cleanImageFileName = imageFileName.Substitute(this.fileNameSubstitutions);
                let imageFilePath = Path.Combine(directoryPath, cleanImageFileName)

                if File.Exists(imageFilePath) = false then
                    let imageDownloader = new FileDownloader()
                    imageDownloader.Download(imageFileName, imageFilePath) |> ignore              

                imageFilePath