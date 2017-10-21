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

        member this.SetAllFileDetails fileSize downloadDate imageFileName =
            if this.FileDetails.FileSize <> fileSize ||
               this.FileDetails.DownloadDate <> downloadDate ||
               this.FileDetails.ImageFileName <> imageFileName then
                    this.FileDetails <-
                    {
                        FileSize = fileSize
                        DownloadDate = downloadDate
                        ImageFileName = imageFileName
                        FileName = this.FileDetails.FileName
                    }

        member this.SetImageFileName imageFileName = 
            if (imageFileName <> this.FileDetails.ImageFileName) then
                this.SetAllFileDetails this.FileDetails.FileSize this.FileDetails.DownloadDate imageFileName

        member this.SetFileDetails fileSize downloadDate = 
            this.SetAllFileDetails fileSize downloadDate this.FileDetails.ImageFileName  
            
        member this.SetFileName (fileName : string) = 
            if (fileName <> this.FileDetails.FileName) then
                this.FileDetails <-
                {
                    FileSize = this.FileDetails.FileSize
                    DownloadDate = this.FileDetails.DownloadDate
                    ImageFileName = this.FileDetails.ImageFileName
                    FileName = fileName
                }   

        override x.Equals other = 
            match other with
            | :? Podcast as y -> (x.Title = y.Title)
            | _ -> false

        // Can be used by other languages to perform equality test on objects of type Podcast
        // This does not work with null references in C# because it is a record (struct) type
        static member op_Equality (x : Podcast, y : Podcast) =
            x.Equals y

        override x.GetHashCode() = hash x.URL

        interface System.IComparable with
            member x.CompareTo other = 
                match other with
                | :? Podcast as y -> compare x y
                | _ -> let message = "Cannot compare values that are not of type Podcast. Type of other is '" + other.GetType().ToString() + "'"
                       invalidArg "other" message