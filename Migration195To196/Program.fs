﻿namespace Migration195To196
// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

module Main = 

    [<EntryPoint>]
    let main argv = 
        FeedMigration.ProcessFile ""
        printfn "%A" argv
        0 // return an integer exit code
