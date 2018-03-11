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

    let finaliseFileNames2 (feedName : string) (podcasts : Podcast[]) : Podcast[] = 

        let finalisingResults = finaliseUsingStandardAlgorithm feedName podcasts
        match (fst finalisingResults) with
        | true -> snd finalisingResults
        | _ ->
            let finalisingResults = finaliseUsingAlternateAlgorithm feedName podcasts
            match (fst finalisingResults) with
            | true -> snd finalisingResults
            | _ -> finaliseUsingDefaultAlgorithm feedName podcasts

    let getSecondLastFragmentFromURL (url : string) : string = 

        let lastSlashIndex = getLastSlashIndex url

        let getURLWithoutLastFragment (url : string) (lastSlashIndex : int) : string = 
            match (lastSlashIndex) with
            | -1 -> url
            | _ -> url.Substring(0, lastSlashIndex)

        let urlWithoutLastFragment = getURLWithoutLastFragment url lastSlashIndex
            
        getLastFragmentFromURL urlWithoutLastFragment

    let finaliseFileNames (feedName : string) (podcasts : Podcast[]) : Podcast[] = 

        let resolvedNamesForEachPodcast = Array.create (Array.length podcasts) ("","")
        let mutable alternativeNames = Set.empty<string>
        let mutable standardNames = Set.empty<string>
        //let mutable standardNames = 0
        //let mutable alternativeNames = 0
        let mutable defaultNames = 0
        let mutable finalNames = Set.empty<string>
        let cleanFeedName = substituteBadFileNameCharacters feedName
        
        let appendFileExtensionIfRequired (name : string) : string = 
            match (name.EndsWith(".mp3")) with
            | false -> name + ".mp3"
            | _ -> name

        let resolveNameUsingStandardAlgorthim (podcast : Podcast) : string =
          getLastFragmentFromURL podcast.URL |> appendFileExtensionIfRequired 

        let resolveNameUsingAlternativeAlgorthim (podcast : Podcast) : string =
          getSecondLastFragmentFromURL podcast.URL |> appendFileExtensionIfRequired 

        let isClash (fileName : string) : bool =
          match (Set.contains fileName finalNames) with
          | false ->
            finalNames <- Set.add fileName finalNames
            false
          | _ ->
            true

        let NoDateTime = DateTime.MinValue.ToUniversalTime()

        let resolveNameUsingDefaultAlgorthim (feedName : string) (podcast : Podcast) : string =
          let fileName = feedName + " - " + podcast.Title + ".mp3"
          match (isClash fileName) with
          | true ->
            match (podcast.PubDate <> NoDateTime) with
            | true ->
              let fileName = feedName + " - " + podcast.PubDate.ToString("dd-MM-yyyy HH-mm-ss") + ".mp3"
              match (isClash fileName) with
              | true ->
                ""
              | _ ->
                finalNames <- Set.add fileName finalNames
                fileName
            | _ ->
              ""
          | _ ->
            finalNames <- Set.add fileName finalNames
            appendFileExtensionIfRequired fileName
        
        let resolveFileNamesForEachPodcast (index: int) (podcast : Podcast) : unit =

          let mutable fileName = ""

          match (String.IsNullOrEmpty podcast.URL) with
          | true -> 
            Array.set resolvedNamesForEachPodcast index ("", "")
          | _ ->
            let standardName = resolveNameUsingStandardAlgorthim podcast
            let alternativeName = resolveNameUsingAlternativeAlgorthim podcast

            standardNames <- Set.add standardName standardNames
            alternativeNames <- Set.add alternativeName alternativeNames

            Array.set resolvedNamesForEachPodcast index (standardName, alternativeName)

        Array.iteri resolveFileNamesForEachPodcast podcasts

        (*let getStandardName (tuple : string * string * string) : string = 
          let t1,_,d = tuple
          match (String.IsNullOrEmpty t1) with
          | false -> t1
          | _ -> d

        let getAlternativeName (tuple : string * string * string) : string = 
          let _,t2,d = tuple
          match (String.IsNullOrEmpty t2) with
          | false -> t2
          | _ -> d*)

        let getFinalName (podcast : Podcast) (name : string) : string = 
          
          match (not <| String.IsNullOrEmpty(name) &&  not <| isClash name) with
          | true ->
            finalNames <- Set.add name finalNames
            name
          | _ ->
            resolveNameUsingDefaultAlgorthim cleanFeedName podcast 

        let useStandardFileNameForEachPodcast (podcast : Podcast) (tuple : string * string) : unit =
          let name,_ = tuple
          getFinalName podcast name |> podcast.SetFileName

        let useAlternativeFileNameForEachPodcast (podcast : Podcast) (tuple : string * string) : unit =
          let _,name = tuple
          getFinalName podcast name |> podcast.SetFileName

        match (Set.count standardNames >= Set.count alternativeNames) with
        | true ->
          Array.iter2 useStandardFileNameForEachPodcast podcasts resolvedNamesForEachPodcast
        | _ ->
          Array.iter2 useAlternativeFileNameForEachPodcast podcasts resolvedNamesForEachPodcast
      
        podcasts
