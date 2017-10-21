namespace Migration195To196
// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.IO

module Main = 

    [<EntryPoint>]
    let main argv = 
        let processFile filePath = 
            try
                printfn "Processing file: %s" filePath
                FeedMigration.processFile filePath
            with
            | e -> printfn "Exception happened during processing of file %s: %s" filePath e.Message
    
    
        printfn "Enter full path to feed directory:"
        let feedPath = Console.ReadLine()

        // Warning: In place file alterations
        Directory.GetFiles(feedPath, "*.feed") |> Array.iter processFile

        printfn "Done."

        Console.ReadLine() |> ignore

        0 // return an integer exit code
