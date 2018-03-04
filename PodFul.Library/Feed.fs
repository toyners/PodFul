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
        ConfirmDownloadThreshold : uint32;
    }

    with
        static member SetProperties title description website directory url imageURL imageFileName podcasts creationDateTime updatedDateTime doScan completeDownloadsOnScan deliverDownloadsOnScan confirmDownloadThreshold =
            {
                Title = title
                Description = description
                Website = website
                Directory = directory
                URL = url
                ImageURL = imageURL
                ImageFileName = imageFileName
                Podcasts = podcasts
                CreationDateTime = creationDateTime
                UpdatedDateTime = updatedDateTime
                DoScan = doScan
                CompleteDownloadsOnScan = completeDownloadsOnScan
                DeliverDownloadsOnScan = deliverDownloadsOnScan
                ConfirmDownloadThreshold = confirmDownloadThreshold
            }

        static member SetImageFileName original (imageFileName : string) = 
            Feed.SetProperties original.Title
                original.Description
                original.Website
                original.Directory
                original.URL
                original.ImageURL
                imageFileName
                original.Podcasts
                original.CreationDateTime
                original.UpdatedDateTime
                original.DoScan
                original.CompleteDownloadsOnScan
                original.DeliverDownloadsOnScan
                original.ConfirmDownloadThreshold

        static member SetUpdatedDate date original =
          Feed.SetProperties
                original.Title
                original.Description
                original.Website
                original.Directory
                original.URL
                original.ImageURL
                original.ImageFileName
                original.Podcasts
                original.CreationDateTime
                date
                original.DoScan
                original.CompleteDownloadsOnScan
                original.DeliverDownloadsOnScan
                original.ConfirmDownloadThreshold

        static member SetDirectory directory original =
            Feed.SetProperties
                original.Title
                original.Description
                original.Website
                directory
                original.URL
                original.ImageURL
                original.ImageFileName
                original.Podcasts
                original.CreationDateTime
                original.UpdatedDateTime
                original.DoScan
                original.CompleteDownloadsOnScan
                original.DeliverDownloadsOnScan
                original.ConfirmDownloadThreshold

        static member SetScanningFlags doScan completeDownloadsOnScan deliverDownloadsOnScan original = 
            Feed.SetProperties
                original.Title
                original.Description
                original.Website
                original.Directory
                original.URL
                original.ImageURL
                original.ImageFileName
                original.Podcasts
                original.CreationDateTime
                original.UpdatedDateTime
                doScan
                completeDownloadsOnScan
                deliverDownloadsOnScan
                original.ConfirmDownloadThreshold

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