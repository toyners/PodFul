namespace PodFul.Library.UnitTests

open System
open System.IO
open NUnit.Framework
open PodFul.Library

type ImageResolver_UnitTests() =

    [<Test>]
    member public this.``Null value returns the default image path``() = 
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath, false, (fun x -> "")) :> IImageResolver

        let actualName = imageResolver.GetName null 

        Assert.AreEqual(actualName, defaultImagePath)

    [<Test>]
    member public this.``Empty string value returns the default image path``() = 
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageResolver = ImageResolver(null, defaultImagePath, false, (fun x -> "")) :> IImageResolver

        let actualName = imageResolver.GetName ""

        Assert.AreEqual(actualName, defaultImagePath)

    [<Test>]
    member public this.``Failed download returns the default image path if options set``() =
        let defaultImagePath = @"C:\DefaultImage.jpg"
        let imageDirectory = @"C:\ImageDirectory\"
        let imageResolver = ImageResolver(imageDirectory, defaultImagePath, true, (fun x -> "")) :> IImageResolver

        let actualName = imageResolver.GetName "Bad image url"

        Assert.AreEqual(actualName, defaultImagePath)