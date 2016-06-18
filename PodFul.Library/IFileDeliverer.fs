namespace PodFul.Library

open System

type IFileDeliverer =

    abstract member Deliver : Podcast -> string -> unit