namespace PodFul.Library

[<AllowNullLiteral>]
type IImageResolver =

    abstract member GetName : string -> string
