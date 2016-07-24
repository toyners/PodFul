namespace PodFul.WPF.UnitTests

open System
open FsUnit
open NUnit.Framework
open PodFul.WPF

/// <summary>
/// Tests the different values that the file size converter must handle. 
/// </summary>
type FileSizeConverter_UnitTests() =

  [<Test>]
  member public this.``Value is null so empty string is returned.``() =
    let converter = new FileSizeConverter()
    converter.Convert(null, null, null, null) |> should equal String.Empty;

  [<Test>]
  member public this.``Value is not of Int64 type so empty string is returned.``() =
    let converter = new FileSizeConverter()
    converter.Convert(new Object(), null, null, null) |> should equal String.Empty;

  [<Test>]
  [<TestCase(0, "0.0 MB")>]
  [<TestCase(1, "~0.0 MB")>]
  [<TestCase(52428, "~0.0 MB")>]
  [<TestCase(104857, "0.1 MB")>]
  [<TestCase(104858, "0.1 MB")>]
  [<TestCase(1048576, "1.0 MB")>]
  [<TestCase(10485760, "10.0 MB")>]
  [<Category("FileSizeConverter_UnitTests")>]
  member public this.``Value is of Int64 type so correct string is returned.``(value : Int64, expectedReturn : string) =
    let converter = new FileSizeConverter()
    converter.Convert(value, null, null, null) |> should equal expectedReturn;