namespace PodFul.Library

open System

[<AllowNullLiteral>]
type IImageResolverOld =

    abstract member PostMessage : Action<string> with get, set

    abstract member GetName : string -> string -> string
