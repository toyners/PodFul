namespace PodFul.WPF.UnitTests

open System
open System.Collections.Generic
open NUnit.Framework
open PodFul.WPF.Logging

type LogController_UnitTests() = 
    
    [<Test>]
    member public this.``Passing a null reference to the cstr throws meaningful exception``() =
        
        let mutable testPassed = false
        try
            let controller = new LogController(null);
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("Parameter 'loggers' is null.", e.Message)
                testPassed <- true  

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing a null reference to a logger to the cstr throws meaningful exception``() =
        
        let mutable testPassed = false
        try
            let controller = new LogController(null);
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("Parameter 'loggers' is null.", e.Message)
                testPassed <- true  

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing non-matching key when displaying message throws a meaningful exception``() =
        
        let mutable testPassed = false
        try
            let controller = new LogController(null);
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("", e.Message)
                testPassed <- true

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing non-matching key when getting logger throws a meaningful exception``() =
        
        let mutable testPassed = false
        try
            let controller = new LogController(null);
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("", e.Message)
                testPassed <- true

        Assert.AreEqual(true, testPassed)
