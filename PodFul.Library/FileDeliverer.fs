namespace PodFul.Library

open System

type FileDeliverer(deliveryPoints : Action<string>[]) =

    let deliveryPoints = deliveryPoints

    interface IFileDeliverer with
        member this.Deliver filePath =
            for deliveryPoint in deliveryPoints do
                deliveryPoint.Invoke(filePath)
