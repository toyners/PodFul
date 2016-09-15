namespace PodFul.Library.UnitTests

open System
open FsUnit
open NUnit.Framework
open PodFul.Library

type ImageResolver_UnitTests() =

    [<Test>]
    member public this.``Null value returns the default image path``() = 
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath, false) :> IImageResolver

        imageResolver.GetName null |> should equal defaultImagePath

    [<Test>]
    member public this.``Empty string value returns the default image path``() = 
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath, false) :> IImageResolver

        imageResolver.GetName "" |> should equal defaultImagePath

    [<Test>]
    member public this.``Failed download returns the default image path if options set``() =
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = @"C:\ImageDirectory\"
        let imageResolver = ImageResolver(imageDirectory, defaultImagePath, true) :> IImageResolver

        imageResolver.GetName "Bad image url" |> should equal defaultImagePath

    [<Test>]
    member public this.``Local path not set and URL path is set so file is downloaded and local path is returned``()=
        ignore
    
    [<Test>]
    member public this.``Local path set and file exists so local path is returned``()=
        ignore

    [<Test>]
    member public this.``Local path not set and URL path not set so default path is returned``()=
        ignore

    [<Test>]
    member public this.``Local path set but file not found locally so is downloaded and local path is returned``()=
        ignore

    [<Test>]
    member public this.``Local name set to default but file not found locally so default name is returned``()=
        ignore

    [<Test>]
    member public this.``Local name set to default and URL name is set so file is downloaded and updated local name is returned``()=
        ignore
