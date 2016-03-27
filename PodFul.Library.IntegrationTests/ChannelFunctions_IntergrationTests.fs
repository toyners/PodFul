namespace PodFul.Library.IntegrationTests

open FsUnit
open NUnit.Framework
open PodFul.Library

type ChannelFunctions_IntergrationTests() = 

    [<Test>]
    member public this.``First Test``() = 
        let x = 1

        x |> should equal 1