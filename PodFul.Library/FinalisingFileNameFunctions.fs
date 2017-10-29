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

    let substituteBadFileNameCharacters (fileName : string) : string = 
        fileName.Substitute(illegalCharacterSubstitutes)

    let defaultPodcastFileName (cleanFeedName : string) (count : int) (index : int) : string =
        (cleanFeedName + " episode " + (count - index).ToString() + ".mp3")

    let finaliseUsingAlgorithm (urlProcessingFunc) (feedName : string) (podcasts : Podcast[]) : bool * Podcast[] =

        let appendFileExtension (name : string) : string = 
            match (name.EndsWith(".mp3")) with
            | false -> name + ".mp3"
            | _ -> name

        let mutable podcastIndex = -1
        let cleanFeedName = substituteBadFileNameCharacters feedName

        let getName (url : string) =
            match (String.IsNullOrEmpty(url)) with
            | true -> defaultPodcastFileName cleanFeedName podcasts.Length podcastIndex
            | _ -> urlProcessingFunc url |> substituteBadFileNameCharacters |> appendFileExtension

        let mutable existingNames = Set.empty

        let setPodcastFileName (podcast : Podcast) : bool = 
            podcastIndex <- podcastIndex + 1
            let name = getName podcast.URL
            let nameNotUsed = not <| Set.contains name existingNames
            let nameClash = false
            match (nameNotUsed) with
            | true -> 
                podcast.SetFileName name
                existingNames <- Set.add name existingNames
                nameNotUsed
            | _ -> nameClash

        let sequenceGenerator index = podcasts.[index]
        
        let resultsSeq = Seq.init podcasts.Length sequenceGenerator |> 
                            Seq.takeWhile setPodcastFileName
        
        let seqCount = Seq.length resultsSeq
        match (podcasts.Length = seqCount) with
        | true -> (true, podcasts)
        | _ -> (false, podcasts)


    let finaliseUsingStandardAlgorithm (feedName : string) (podcasts : Podcast[]) : bool * Podcast[] = 
        finaliseUsingAlgorithm getLastFragmentFromURL feedName podcasts

    let finaliseUsingAlternateAlgorithm (feedName : string) (podcasts : Podcast[]) : bool * Podcast[] = 

        let getSecondLastFragmentFromURL (url : string) : string = 

            let lastSlashIndex = getLastSlashIndex url

            let getURLWithoutLastFragment (url : string) (lastSlashIndex : int) : string = 
                match (lastSlashIndex) with
                | -1 -> url
                | _ -> url.Substring(0, lastSlashIndex)

            let urlWithoutLastFragment = getURLWithoutLastFragment url lastSlashIndex
            
            getLastFragmentFromURL urlWithoutLastFragment

        finaliseUsingAlgorithm getSecondLastFragmentFromURL feedName podcasts

    let finaliseUsingDefaultAlgorithm (feedName : string) (podcasts : Podcast[]) : Podcast[] = 

        let cleanFeedName = substituteBadFileNameCharacters feedName 
        let setPodcastFileName (index : int) (podcast : Podcast) = 
            defaultPodcastFileName cleanFeedName podcasts.Length index |> podcast.SetFileName

        let sequenceGenerator index = podcasts.[index]
        let resultsSeq = Seq.init podcasts.Length sequenceGenerator |> 
                            Seq.iteri setPodcastFileName

        podcasts

    let finaliseFileNames (feedName : string) (podcasts : Podcast[]) : Podcast[] = 

        let finalisingResults = finaliseUsingStandardAlgorithm feedName podcasts
        match (fst finalisingResults) with
        | true -> snd finalisingResults
        | _ ->
            let finalisingResults = finaliseUsingAlternateAlgorithm feedName podcasts
            match (fst finalisingResults) with
            | true -> snd finalisingResults
            | _ -> finaliseUsingDefaultAlgorithm feedName podcasts