namespace PodFul.Library

open System

type IFileDeliverer =

    abstract member Deliver : string -> unit