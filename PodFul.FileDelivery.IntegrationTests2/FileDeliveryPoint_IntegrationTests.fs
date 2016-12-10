namespace PodFul.FileDelivery.IntegrationTests

open System
open System.IO
open Jabberwocky.Toolkit.IO
open NUnit.Framework
open PodFul.FileDelivery

type FileDeliveryPoint_IntegrationTests() =

    let mutable workingDirectory = String.Empty;

    [<SetUp>]
    member public this.SetupBeforeEachTest() =
        workingDirectory <- Path.Combine(Path.GetTempPath(), "FileDeliveryPoint_IntegrationTests")
        DirectoryOperations.EnsureDirectoryIsEmpty(workingDirectory) 

    [<Test>]
    member public this.Test() =
        let filePath = Path.Combine(workingDirectory, Path.GetTempFileName())
        let destinationPath = Path.Combine(workingDirectory, "Destination")
        let fileDeliveryPoint = new FileDeliveryPoint(destinationPath, )

        fileDeliveryPoint.Deliver(filePath, "file")




