namespace PodFul.WPF.UnitTests

open System
open System.Collections.Generic
open FsUnit
open NUnit.Framework
open PodFul.WPF.Logging

/// <summary>
/// Tests the different scenarios for creating delivery point instances.
/// </summary>
type DeliveryPointCreator_UnitTests() =

    [<Test>]
    member public this.``Delivery Point data list is null so no delivery points are created.``() =
        let instances = PodFul.WPF.Miscellaneous.DeliveryPointCreator.CreateDeliveryPoints(null, new FileDeliveryLogger(), new FileDeliveryLogger())
        
        instances |> should not' (equal null)
        instances.Length |> should equal 0

    [<Test>]
    member public this.``Delivery Point data list is empty so no delivery points are created.``() =
        
        let deliveryPointData = new List<PodFul.WPF.Miscellaneous.Settings.SettingsData.DeliveryPointData>()
        let instances = PodFul.WPF.Miscellaneous.DeliveryPointCreator.CreateDeliveryPoints(deliveryPointData, new FileDeliveryLogger(), new FileDeliveryLogger())
        
        instances |> should not' (equal null)
        instances.Length |> should equal 0

    [<Test>]
    member public this.``Delivery Point data list contains disabled point only so no delivery points are created.``() =

        let disabledDeliveryPoint = PodFul.WPF.Miscellaneous.Settings.SettingsData.DeliveryPointData()
        let deliveryPointData = new List<PodFul.WPF.Miscellaneous.Settings.SettingsData.DeliveryPointData>();
        deliveryPointData.Add(disabledDeliveryPoint)

        let instances = PodFul.WPF.Miscellaneous.DeliveryPointCreator.CreateDeliveryPoints(deliveryPointData, new FileDeliveryLogger(), new FileDeliveryLogger())
        
        instances |> should not' (equal null)
        instances.Length |> should equal 0

    [<Test>]
    member public this.``Delivery Point data list contains one point so one delivery point is created.``() =

        let deliveryPoint = PodFul.WPF.Miscellaneous.Settings.SettingsData.DeliveryPointData()
        deliveryPoint.Enabled <- true
        deliveryPoint.Location <- @"C:\Location\File.exe"
        deliveryPoint.Type <- PodFul.WPF.Miscellaneous.Settings.SettingsData.DeliveryPointData.Types.Winamp

        let deliveryPointData = new List<PodFul.WPF.Miscellaneous.Settings.SettingsData.DeliveryPointData>();
        deliveryPointData.Add(deliveryPoint)

        let instances = PodFul.WPF.Miscellaneous.DeliveryPointCreator.CreateDeliveryPoints(deliveryPointData, new FileDeliveryLogger(), new FileDeliveryLogger())
        
        instances |> should not' (equal null)
        instances.Length |> should equal 1
