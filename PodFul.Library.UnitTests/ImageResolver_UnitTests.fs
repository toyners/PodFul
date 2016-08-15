namespace PodFul.Library.UnitTests

open System
open FsUnit
open NUnit.Framework
open PodFul.Library

type ImageResolver_UnitTests() =

    [<Test>]
    member public this.``Null value returns the default image path``() = 
        let imageDirectory = @""
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath, null) :> IImageResolver

        imageResolver.GetName null |> should equal defaultImagePath

    [<Test>]
    member public this.``Empty string value returns the default image path``() = 
        let imageDirectory = @""
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath, null) :> IImageResolver

        imageResolver.GetName "" |> should equal defaultImagePath
