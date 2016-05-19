namespace PodFul.Library

open System

type IFeedStorage =

    abstract member Feeds : Feed[]

    abstract member IsOpen : Boolean

    abstract member Open : unit -> unit

    abstract member Update : Feed -> unit
