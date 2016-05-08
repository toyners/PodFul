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
        Podcasts : Podcast[];
    }

    with
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