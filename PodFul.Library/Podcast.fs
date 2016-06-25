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
        ImageFileName : string;
    }

    with
        static member SetDownloadDate date original =
            {
                Title = original.Title
                Description = original.Description
                URL = original.URL
                FileSize = original.FileSize
                PubDate = original.PubDate
                DownloadDate = date
                ImageFileName = original.ImageFileName
            }

        static member SetFileSize size original =
            {
                Title = original.Title
                Description = original.Description
                URL = original.URL
                FileSize = size
                PubDate = original.PubDate
                DownloadDate = original.DownloadDate
                ImageFileName = original.ImageFileName
            }

        static member SetImageFileName imageFileName original = 
            {
                Title = original.Title
                Description = original.Description
                URL = original.URL
                FileSize = original.FileSize
                PubDate = original.PubDate
                DownloadDate = original.DownloadDate
                ImageFileName = imageFileName
            }

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