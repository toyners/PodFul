namespace PodFul.Library

open System

[<AllowNullLiteral>]
type IImageResolver =

    abstract member PostMessage : Action<string> with get, set

    abstract member GetName : string -> string
