namespace PodFul.Library

open System

type IFileDelivereryPoint =

    abstract member Title : string with get

    abstract member Deliver : string -> unit