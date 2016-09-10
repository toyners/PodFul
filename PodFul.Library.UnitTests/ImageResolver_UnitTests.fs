namespace PodFul.Library.UnitTests

open System
open FsUnit
open NUnit.Framework
open PodFul.Library

type ImageResolver_UnitTests() =

    [<Test>]
    member public this.``Null value returns the default image path``() = 
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath) :> IImageResolver

        imageResolver.GetName null |> should equal defaultImagePath

    [<Test>]
    member public this.``Empty string value returns the default image path``() = 
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath) :> IImageResolver

        imageResolver.GetName "" |> should equal defaultImagePath

    [<Test>]
    member public this.``Failed download returns the default image path if options set``() =
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver("", defaultImagePath) :> IImageResolver
        imageResolver.ReturnDefaultImageOnException <- true

        imageResolver.GetName "Bad image url" |> should equal defaultImagePath

