namespace PodFul.Library

open System

type FileDeliverer(deliveryPoints : Action<Podcast, string>[]) =

    let deliveryPoints = deliveryPoints

    interface IFileDeliverer with
        member this.Deliver podcast filePath =
            for deliveryPoint in deliveryPoints do
                deliveryPoint.Invoke(podcast, filePath)
