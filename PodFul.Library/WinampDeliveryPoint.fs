namespace PodFul.Library

open System
open System.IO

type WinampDeliveryPoint(postMessage : Action<string>) =

    let postMessage = postMessage

    member this.DeliverToWinamp(filePath : string) = ignore
