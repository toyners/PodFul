namespace PodFul.Library

open Jabberwocky.Toolkit.String
open System
open System.Collections.Generic

module public FinalisingFileNameFunctions = 

    let illegalCharacterSubstitutes = Dictionary<string, string>(dict
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

    let finaliseUsingStandardAlgorithm (podcasts : list<Podcast>) : bool * list<Podcast> = 

        let createDefaultFileName (fileCount : int) =
            "file_" + DateTime.Now.ToString("ddMMyyyy") + "_" + fileCount.ToString() + ".mp3"

        let getSlashIndex (url : string) =
            let result = url.LastIndexOf('/') + 1
            match (result) with
            | 0 -> url.LastIndexOf('\\') + 1
            | _ -> result

        let getLastFragmentFromURL (url : string) =
            let slashIndex = getSlashIndex url
            let mp3Index = url.LastIndexOf(".mp3")
            let mp3IndexGreaterThanSlashIndex = mp3Index - slashIndex > 1
            match (mp3IndexGreaterThanSlashIndex) with
            | true -> url.Substring(slashIndex, mp3Index - slashIndex + ".mp3".Length)
            | _ -> url.Substring(slashIndex)

        let appendFileExtension (name : string) : string = 
            match (name.EndsWith(".mp3")) with
            | false -> name + ".mp3"
            | _ -> name

        let mutable existingNames = Set.empty

        let getName (url : string) =
            match (String.IsNullOrEmpty(url)) with
            | true -> createDefaultFileName ((Set.count existingNames) + 1)
            | _ -> getLastFragmentFromURL url |> appendFileExtension

        let setPodcastFileName (podcast : Podcast) : bool = 
            let name = getName podcast.URL
            let b = Set.contains name existingNames
            match (b) with
            | false -> 
                podcast.SetFileName name
                existingNames <- Set.add name existingNames
                true
            | _ -> false

        let sequenceGenerator index = podcasts.Item index
        
        let resultsSeq = Seq.init podcasts.Length sequenceGenerator |> 
                            Seq.takeWhile setPodcastFileName
        
        let seqCount = Seq.length resultsSeq
        match (podcasts.Length = seqCount) with
        | true -> (true, podcasts)
        | _ -> (false, podcasts)

    let finaliseUsingAlternateAlgorithm (podcasts : list<Podcast>) : bool * list<Podcast> = 

        let createDefaultFileName (fileCount : int) =
            "file_" + DateTime.Now.ToString("ddMMyyyy") + "_" + fileCount.ToString() + ".mp3"

        let sequenceGenerator index = podcasts.Item index

        let appendFileExtension (name : string) : string = 
            match (name.EndsWith(".mp3")) with
            | false -> name + ".mp3"
            | _ -> name

        let getSecondLastFragmentFromURL (url : string) : string = 
            raise (new System.NotImplementedException())

        let mutable existingNames = Set.empty

        let getName (url : string) =
            match (String.IsNullOrEmpty(url)) with
            | true -> createDefaultFileName ((Set.count existingNames) + 1)
            | _ -> getSecondLastFragmentFromURL url |> appendFileExtension

        let setPodcastFileName (podcast : Podcast) : bool = 
            let name = getName podcast.URL
            let b = Set.contains name existingNames
            match (b) with
            | false -> 
                podcast.SetFileName name
                existingNames <- Set.add name existingNames
                true
            | _ -> false

        let resultsSeq = Seq.init podcasts.Length sequenceGenerator |> 
                            Seq.takeWhile setPodcastFileName
        
        let seqCount = Seq.length resultsSeq
        match (podcasts.Length = seqCount) with
        | true -> (true, podcasts)
        | _ -> (false, podcasts)

    let substituteBadFileNameCharacters (fileName : string) : string = 
        fileName.Substitute(illegalCharacterSubstitutes)