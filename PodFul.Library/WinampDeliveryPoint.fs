﻿namespace PodFul.Library

open System
open System.IO

type WinampDeliveryPoint(winampExePath : string, postMessage : Action<string>, postException : Action<string>) =

    let winampExePath = winampExePath //@"C:\Program Files (x86)\Winamp\winamp.exe"
    let postMessage = postMessage
    let postException = postException;

    member this.DeliverToWinamp (podcast : Podcast) (filePath : string) =
        
        try
            let arguments = "/ADD \"" + filePath + "\""
            System.Diagnostics.Process.Start(winampExePath, arguments) |> ignore
            postMessage.Invoke("Added \"" + podcast.Title + "\" to WinAmp.")
        with ex ->
            postException.Invoke("Failed to add \"" + podcast.Title + "\" to Winamp: " + ex.Message)
