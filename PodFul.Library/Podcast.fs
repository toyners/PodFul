namespace PodFul.Library

open System

[<CustomEquality; CustomComparison>]
type Podcast =
    {
        Title : string;
        Description : string;
        URL : string;
        FileSize : Int64;
        PubDate : DateTime;
        DownloadDate : DateTime;
    }

    with
        static member SetDownloadDate original date =
            {
                Title = original.Title
                Description = original.Description
                URL = original.URL
                FileSize = original.FileSize
                PubDate = original.PubDate
                DownloadDate = date
            }

        static member SetFileSize original size =
            {
                Title = original.Title
                Description = original.Description
                URL = original.URL
                FileSize = size
                PubDate = original.PubDate
                DownloadDate = original.DownloadDate
            }

        override x.Equals other = 
            match other with
            | :? Podcast as y -> (x.URL = y.URL)
            | _ -> false

        override x.GetHashCode() = hash x.URL

        interface System.IComparable with
            member x.CompareTo other = 
                match other with
                | :? Podcast as y -> compare x y
                | _ -> let message = "Cannot compare values that are not of type Podcast. Type of other is '" + other.GetType().ToString() + "'"
                       invalidArg "other" message