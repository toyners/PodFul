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
            new LogController(null) |> ignore
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("Parameter 'loggers' is null.", e.Message)
                testPassed <- true  

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing a null logger reference in collection to the cstr throws meaningful exception``() =
        
        let mutable testPassed = false
        let loggers = new Dictionary<String, ILogger>()
        loggers.Add("Key", null)

        try
            new LogController(loggers) |> ignore
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("Parameter 'loggers' contains null value reference.", e.Message)
                testPassed <- true  

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing non-matching key when displaying message throws a meaningful exception``() =
        
        let mutable testPassed = false
        let logger = new FileLogger()
        let loggers = new Dictionary<String, ILogger>()
        loggers.Add("Log", logger)
        let logController = new LogController(loggers)

        try
            logController.Message("Key", "Message")
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("LogController does not have Logger matching key 'Key'.", e.Message)
                testPassed <- true

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing non-matching key when getting logger throws a meaningful exception``() =
        
        let mutable testPassed = false
        let logger = new FileLogger()
        let loggers = new Dictionary<String, ILogger>()
        loggers.Add("Log", logger)
        let controller = new LogController(loggers);

        try
            controller.GetLogger("Key") |> ignore
            testPassed <- false
        with
            | e -> 
                Assert.AreEqual("LogController does not have Logger matching key 'Key'.", e.Message)
                testPassed <- true

        Assert.AreEqual(true, testPassed)

    [<Test>]
    member public this.``Passing matching key when getting logger returns logger``() =
        
        let expected = new FileLogger()
        let loggers = new Dictionary<String, ILogger>()
        loggers.Add("Key", expected)
        let controller = new LogController(loggers);
        
        let actual = controller.GetLogger("Key")

        Assert.AreEqual(expected, actual)

    [<Test>]
    member public this.``Passing matching key when getting logger of type returns logger``() =
        
        let expected = new FileLogger()
        let loggers = new Dictionary<String, ILogger>()
        loggers.Add("Key", expected)
        let controller = new LogController(loggers);
        
        let actual = controller.GetLogger<FileLogger>("Key")

        Assert.AreEqual(expected, actual)

    [<Test>]
    member public this.``Passing matching key with wrong type parameter when getting logger throws a meaningful exception``() =
        
        let loggers = new Dictionary<String, ILogger>()
        loggers.Add("Key", new FileLogger())
        let controller = new LogController(loggers);
        let mutable testPassed = false

        try
            let actual = controller.GetLogger<UILogger>("Key")
            testPassed <- false
        with
        | e ->
            Assert.AreEqual("Type of value (PodFul.WPF.Logging.FileLogger) does not match parameter type (PodFul.WPF.Logging.UILogger).", e.Message)
            testPassed <- true

        Assert.AreEqual(true, testPassed)
