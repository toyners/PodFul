namespace PodFul.Library

open System
open System.IO

module public ImageFunctions =

    let needsToDownloadImage (podcast : Podcast) (defaultImagePath : string) : bool =
        let localName = podcast.FileDetails.ImageFileName
        let gotLocalPath = String.IsNullOrEmpty(localName) = false
        let gotURLPath = String.IsNullOrEmpty(podcast.ImageURL) = false

        ((gotLocalPath = false || localName = defaultImagePath) && gotURLPath) ||
            (gotLocalPath && File.Exists(localName) = false)

    let determineImageDownloadCount (podcasts : Podcast[]) (defaultImagePath : string) : int =
        let mutable index = 0
        let mutable count = 0
        while index < podcasts.Length do
            
            if needsToDownloadImage podcasts.[index] "" then
                count <- count + 1
            index <- index + 1

        count

    let resolveImages (podcasts : Podcast[]) resolveLocalFilePathFunction =
        // Get count of images that need downloading for collection of podcasts
        let mutable index = 0
        while index < podcasts.Length do
            let podcast = podcasts.[index]

            let localPath = resolveLocalFilePathFunction podcast.ImageURL

            //if needsToDownloadImage localPath podcast.ImageURL

            index <- index + 1

        // If not downloading images use default image

        // iterate over podcasts and
        // resolve name to local file name
        // if local file exists then use it
        // if local file does not exist then
        //      try download image
        //      if allowed return local file name
        //      if not allowed then
        //          try use default image
        //          if passed in return default image name
        //          if not passed in set name to blank
        //      if allowed and failed then
        //          Log
        //          try use default image        
        //          if passed in set default image name
        //          if not passed in return blank
        ()