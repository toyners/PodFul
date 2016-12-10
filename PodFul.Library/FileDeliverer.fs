namespace PodFul.Library

open System

type FFileDeliverer(deliveryPoints : Action<Podcast, string>[]) =

    let deliveryPoints = deliveryPoints

    interface IIFileDeliverer with
        member this.Deliver podcast filePath =
            for deliveryPoint in deliveryPoints do
                deliveryPoint.Invoke(podcast, filePath)
