namespace PodFul.WPF.UnitTests

open System
open System.Collections.Generic
open FsUnit
open NUnit.Framework
open PodFul.WPF.Logging

type LogController_UnitTests() = 
    
    [<Test>]
    member public this.``test``() =
        
        let mutable testPassed = false
        try
            let controller = new LogController(null);
            testPassed <- false
        with
            | ex -> 
                let message = ex.Message
                if message = "" then
                    testPassed <- true  

        testPassed |> should equal true


