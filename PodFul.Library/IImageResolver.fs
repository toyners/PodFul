namespace PodFul.Library

open System

[<AllowNullLiteral>]
type IImageResolver =

    abstract member GetName : string -> string

    abstract member PostMessage : Action<string> with get, set
