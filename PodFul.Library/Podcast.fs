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