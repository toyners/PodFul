namespace PodFul.Library

type ImageResolver(imageDirectoryPath : string) =

    let directoryPath = imageDirectoryPath

    interface IImageResolver with
        
        member this.GetName (imageFileName : string) : string = 
            ""