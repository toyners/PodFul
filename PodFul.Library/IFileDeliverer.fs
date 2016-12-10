namespace PodFul.Library

open System

type IIFileDeliverer =

    abstract member Deliver : Podcast -> string -> unit