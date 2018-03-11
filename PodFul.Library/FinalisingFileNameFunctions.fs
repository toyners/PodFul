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

        let resolveNameUsingDefaultAlgorthim (feedName : string) (podcast : Podcast) : string =
          let fileName = feedName + " - " + podcast.Title + ".mp3"
          match (isClash fileName) with
          | true ->
            match (podcast.PubDate <> Miscellaneous.NoDateTime) with
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
          | true -> // url is empty
            Array.set resolvedNamesForEachPodcast index ("", "")
          | _ -> // url is not empty
            let standardName = resolveNameUsingStandardAlgorthim podcast
            let alternativeName = resolveNameUsingAlternativeAlgorthim podcast

            standardNames <- Set.add standardName standardNames
            alternativeNames <- Set.add alternativeName alternativeNames

            Array.set resolvedNamesForEachPodcast index (standardName, alternativeName)

        Array.iteri resolveFileNamesForEachPodcast podcasts

        let getFinalName (podcast : Podcast) (name : string) : string = 
          
          match (not <| String.IsNullOrEmpty(name) &&  not <| isClash name) with
          | true -> // name is not empty and not clashing with an existing name
            finalNames <- Set.add name finalNames
            name
          | _ -> // name is empty or clashes with an existing name
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
