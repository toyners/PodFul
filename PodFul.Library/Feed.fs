namespace PodFul.Library

open System

[<CustomEquality; CustomComparison>]
type Feed = 
    {
        Title : string;
        Description : string;
        Website : string;
        Directory : string;
        URL : string;
        ImageURL : string;
        ImageFileName : string;
        Podcasts : Podcast[];
        CreationDateTime : DateTime
        UpdatedDateTime : DateTime
        DoScan : bool
        CompleteDownloadsOnScan : bool
        DeliverDownloadsOnScan : bool
    }

    with

        static member SetImageFileName original (imageFileName : string) = 
            {
                Title = original.Title
                Description = original.Description
                Website = original.Website
                Directory = original.Directory
                URL = original.URL
                ImageURL = original.ImageURL
                ImageFileName = imageFileName
                Podcasts = original.Podcasts
                CreationDateTime = original.CreationDateTime
                UpdatedDateTime = original.UpdatedDateTime
                DoScan = original.DoScan
                CompleteDownloadsOnScan = original.CompleteDownloadsOnScan
                DeliverDownloadsOnScan = original.DeliverDownloadsOnScan
            }

        static member SetUpdatedDate date original =
            {
                Title = original.Title
                Description = original.Description
                Website = original.Website
                Directory = original.Directory
                URL = original.URL
                ImageURL = original.ImageURL
                ImageFileName = original.ImageFileName
                Podcasts = original.Podcasts
                CreationDateTime = original.CreationDateTime
                UpdatedDateTime = date
                DoScan = original.DoScan
                CompleteDownloadsOnScan = original.CompleteDownloadsOnScan
                DeliverDownloadsOnScan = original.DeliverDownloadsOnScan
            }

        static member SetDirectory directory original =
            {
                Title = original.Title
                Description = original.Description
                Website = original.Website
                Directory = directory
                URL = original.URL
                ImageURL = original.ImageURL
                ImageFileName = original.ImageFileName
                Podcasts = original.Podcasts
                CreationDateTime = original.CreationDateTime
                UpdatedDateTime = original.UpdatedDateTime
                DoScan = original.DoScan
                CompleteDownloadsOnScan = original.CompleteDownloadsOnScan
                DeliverDownloadsOnScan = original.DeliverDownloadsOnScan
            }

        static member SetScanningFlags doScan completeDownloadsOnScan deliverDownloadsOnScan original = 
            {
                Title = original.Title
                Description = original.Description
                Website = original.Website
                Directory = original.Directory
                URL = original.URL
                ImageURL = original.ImageURL
                ImageFileName = original.ImageFileName
                Podcasts = original.Podcasts
                CreationDateTime = original.CreationDateTime
                UpdatedDateTime = original.UpdatedDateTime
                DoScan = doScan
                CompleteDownloadsOnScan = completeDownloadsOnScan
                DeliverDownloadsOnScan = deliverDownloadsOnScan
            }

        override x.Equals other = 
            match other with
            | :? Feed as y -> (x.URL = y.URL)
            | _ -> false

        override x.GetHashCode() = hash x.URL

        interface System.IComparable with
            member x.CompareTo other = 
                match other with
                | :? Feed as y -> compare x y
                | _ -> let message = "Cannot compare values that are not of type Feed. Type of other is '" + other.GetType().ToString() + "'"
                       invalidArg "other" message