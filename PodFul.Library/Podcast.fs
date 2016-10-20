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
                    let mutable slashIndex = this.URL.LastIndexOf('/') + 1
                    if slashIndex = 0 then
                        slashIndex <- this.URL.LastIndexOf('\\') + 1

                    // Handle the situation where the file name is buried in the url 
                    // e.g. http://abc.com/filename.mp3?somekey=somevalue.
                    // But watch out for situations where .mp3 is part of the domain 
                    // e.g. http://abc.mp3.cpm/filename
                    let mp3Index = this.URL.LastIndexOf(".mp3")

                    let mutable name = null
                    if mp3Index - slashIndex > 1 then
                        name <- this.URL.Substring(slashIndex, mp3Index - slashIndex + ".mp3".Length)
                    else
                        name <- this.URL.Substring(slashIndex)

                    // Ensure that the filename extension is present.
                    if name.EndsWith(".mp3") <> true then
                        name <- name + ".mp3"

                    name.Substitute(this.fileNameSubstitutions)

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