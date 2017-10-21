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

    let getLastSlashIndex (url : string) =
        let result = url.LastIndexOf('/')
        match (result) with
        | -1 -> url.LastIndexOf('\\')
        | _ -> result

    let getLastFragmentFromURL (url : string) =
        let firstCharacterOfFragmentIndex = (getLastSlashIndex url) + 1
        let mp3Index = url.LastIndexOf(".mp3")
        let mp3IndexGreaterThanSlashIndex = mp3Index - firstCharacterOfFragmentIndex > 1
        match (mp3IndexGreaterThanSlashIndex) with
        | true -> url.Substring(firstCharacterOfFragmentIndex, mp3Index - firstCharacterOfFragmentIndex + ".mp3".Length)
        | _ -> url.Substring(firstCharacterOfFragmentIndex)

    let finaliseUsingAlgorithm (urlProcessingFunc) (podcasts : list<Podcast>) : bool * list<Podcast> =

        let createDefaultFileName (fileCount : int) =
            "file_" + DateTime.Now.ToString("ddMMyyyy") + "_" + fileCount.ToString() + ".mp3"

        let appendFileExtension (name : string) : string = 
            match (name.EndsWith(".mp3")) with
            | false -> name + ".mp3"
            | _ -> name

        let mutable existingNames = Set.empty

        let getName (url : string) =
            match (String.IsNullOrEmpty(url)) with
            | true -> createDefaultFileName ((Set.count existingNames) + 1)
            | _ -> urlProcessingFunc url |> appendFileExtension

        let setPodcastFileName (podcast : Podcast) : bool = 
            let name = getName podcast.URL
            let nameNotUsed = not <| Set.contains name existingNames
            match (nameNotUsed) with
            | true -> 
                podcast.SetFileName name
                existingNames <- Set.add name existingNames
                nameNotUsed
            | _ -> nameNotUsed

        let sequenceGenerator index = podcasts.Item index
        
        let resultsSeq = Seq.init podcasts.Length sequenceGenerator |> 
                            Seq.takeWhile setPodcastFileName
        
        let seqCount = Seq.length resultsSeq
        match (podcasts.Length = seqCount) with
        | true -> (true, podcasts)
        | _ -> (false, podcasts)


    let finaliseUsingStandardAlgorithm (podcasts : list<Podcast>) : bool * list<Podcast> = 
        finaliseUsingAlgorithm getLastFragmentFromURL podcasts

    let finaliseUsingAlternateAlgorithm (podcasts : list<Podcast>) : bool * list<Podcast> = 

        let getSecondLastFragmentFromURL (url : string) : string = 

            let lastSlashIndex = getLastSlashIndex url

            let getURLWithoutLastFragment (url : string) (lastSlashIndex : int) : string = 
                match (lastSlashIndex) with
                | -1 -> url
                | _ -> url.Substring(0, lastSlashIndex)

            let urlWithoutLastFragment = getURLWithoutLastFragment url lastSlashIndex
            
            getLastFragmentFromURL urlWithoutLastFragment

        finaliseUsingAlgorithm getSecondLastFragmentFromURL podcasts

    let substituteBadFileNameCharacters (fileName : string) : string = 
        fileName.Substitute(illegalCharacterSubstitutes)

    let finaliseFileNames (feedName : string) (podcasts : list<Podcast>) : bool * list<Podcast> = 
        raise (new System.NotImplementedException())