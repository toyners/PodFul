namespace PodFul.WPF.UnitTests

open System
open System.Collections.Generic
open NUnit.Framework
open PodFul.WPF.Logging

type LogController_UnitTests() = 
    
    [<Test>]
    member public this.``Passing a null collection reference to the cstr throws meaningful exception``() =
        
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
    member public this.``Passing a null logger reference in collection to the cstr throws meaningful exception``() =
        
        let mutable testPassed = false
        try
            let loggers = new Dictionary<String, ILogger>()
            loggers.Add("Key", null)
            let controller = new LogController(loggers)
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("Parameter 'loggers' contains null value reference.", e.Message)
                testPassed <- true  

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing a null key reference in collection to the cstr throws meaningful exception``() =
        
        let mutable testPassed = false
        try
            let logger = new FileLogger()
            let loggers = new Dictionary<String, ILogger>()
            loggers.Add(null, logger)
            let controller = new LogController(loggers)
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("Parameter 'loggers' contains null key reference.", e.Message)
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
    member public this.``Passing non-matching key when displaying exception throws a meaningful exception``() =
        
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
