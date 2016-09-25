namespace PodFul.Library.UnitTests

open NUnit.Framework
open PodFul.Library

type Miscellaneous_UnitTests() =

    [<Test>]
    [<TestCase(".jpg")>]
    [<TestCase(".JPG")>]
    member public this.``Image name has same file extension as URL path``(expectedExtension : string) =
        let urlPath = "http://image" + expectedExtension
        let result = Miscellaneous.NextImageFileName urlPath

        Assert.AreEqual(result.EndsWith(expectedExtension), true)

    [<Test>]
    member public this.``Image name has no file extension if URL path has no extension``() =
        let urlPath = "http://image"
        let result = Miscellaneous.NextImageFileName urlPath

        Assert.AreEqual(result.Contains("."), false)

