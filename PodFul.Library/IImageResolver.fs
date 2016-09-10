namespace PodFul.Library

open System

[<AllowNullLiteral>]
type IImageResolver =

    abstract member ReturnDefaultImageOnException : Boolean with get, set

    abstract member PostMessage : Action<string> with get, set

    abstract member GetName : string -> string
