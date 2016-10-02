namespace PodFul.Library

open System
open System.Collections.Generic
open Jabberwocky.Toolkit.String

[<CustomEquality; CustomComparison>]
type Podcast =
    {
        Title : string;
        Description : string;
        URL : string;
        ImageURL : string;
        PubDate : DateTime;
        mutable FileDetails : PodcastFile;
    }

    with

        member private this.fileNameSubstitutions = Dictionary<string, string>(dict
                                                        [
                                                            ( "\\", "-bs-" );
                                                            ( "/", "-fs-" );
                                                            ( ":", "-c-" );
                                                            ( "*", "-a-" );
                                                            ( "?", "-q-" );
                                                            ( "\"", "-qu-" );
                                                            ( "<", "-lt-" );
                                                            ( ">", "-gt-" );
                                                            ( "|", "-b-" )
                                                        ])

        member this.FileName 
            with get() = 
                match(String.IsNullOrEmpty(this.URL)) with
                | true -> 
                    failwith "Cannot get FileName: URL is null or empty."
                | _ ->
                    let mutable index = this.URL.LastIndexOf('/') + 1
                    if index = 0 then
                        index <- this.URL.LastIndexOf('\\') + 1

                    this.URL.Substring(index).Substitute(this.fileNameSubstitutions)

        member this.SetAllFileDetails fileSize downloadDate imageFileName =
            if this.FileDetails.FileSize <> fileSize ||
               this.FileDetails.DownloadDate <> downloadDate ||
               this.FileDetails.ImageFileName <> imageFileName then
                    this.FileDetails <-
                    {
                        FileSize = fileSize
                        DownloadDate = downloadDate
                        ImageFileName = imageFileName
                    }

        member this.SetImageFileName imageFileName = 
            if (imageFileName <> this.FileDetails.ImageFileName) then
                this.SetAllFileDetails this.FileDetails.FileSize this.FileDetails.DownloadDate imageFileName

        member this.SetFileDetails fileSize downloadDate = 
            this.SetAllFileDetails fileSize downloadDate this.FileDetails.ImageFileName        

        override x.Equals other = 
            match other with
            | :? Podcast as y -> (x.Title = y.Title)
            | _ -> false

        override x.GetHashCode() = hash x.URL

        interface System.IComparable with
            member x.CompareTo other = 
                match other with
                | :? Podcast as y -> compare x y
                | _ -> let message = "Cannot compare values that are not of type Podcast. Type of other is '" + other.GetType().ToString() + "'"
                       invalidArg "other" message