﻿namespace PodFul.Library

open System
open System.IO

module public Miscellaneous = 

    let combine x y = Path.Combine(x, y)

    let public GetReadableFileSize (rawSize : Double) : string = 
        
        let sizeInMb = rawSize / (1048576.0)

        sizeInMb.ToString("#0.0")

    let public SwapDirectoryInFilePath directoryPath (filePath : string) =
        filePath.LastIndexOf('/') + 1 |> filePath.Substring |> combine directoryPath

    let public OldNextImageFileName (urlPath : string) =
        let mutable name = Guid.NewGuid().ToString()
        if urlPath.EndsWith(".jpg", true, null) then
            name <- name + ".jpg"
        name
        