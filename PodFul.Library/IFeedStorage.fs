namespace PodFul.Library

open System

type IFeedStorage =

    abstract member Feeds : Feed[] with get

    abstract member IsOpen : Boolean with get

    abstract member Add : Feed -> unit

    abstract member Close : unit -> unit

    abstract member Open : unit -> unit

    abstract member Remove : Feed -> unit

    abstract member Update : Feed -> unit

