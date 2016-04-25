namespace PodFul.Library

open System

module public Miscellaneous = 

    let public GetReadableFileSize (rawSize : Double) : string = 
        
        let sizeInMb = rawSize / (1048576.0)

        sizeInMb.ToString("00.0")