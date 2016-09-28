namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library

type Miscellaneous_UnitTests() =

    [<Test>]
    [<TestCase(".jpg")>]
    [<TestCase(".JPG")>]
    [<TestCase(".jPG")>]
    member public this.``Image name has same JPG extension if URL path has JPG extension``(expectedExtension : string) =
        let urlPath = "http://image" + expectedExtension
        let result = Miscellaneous.NextImageFileName urlPath

        Assert.AreEqual(result.EndsWith("JPG", true, null), true)

    [<Test>]
    member public this.``Image name has no extension if URL path has no JPG extension``() =
        let urlPath = "http://image"
        let result = Miscellaneous.NextImageFileName urlPath

        Assert.AreEqual(result.EndsWith("JPG", true, null), false)

    [<Test>]
    member public this.``Image name has no JPG extension if URL path has no extension but does have a dot``() =
        let urlPath = "http://image.name"
        let result = Miscellaneous.NextImageFileName urlPath

        Assert.AreEqual(result.EndsWith("JPG", true, null), false)

