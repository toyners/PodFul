namespace PodFul.Library

open System
open System.IO
open System.Net

type File1() =

    let download url filePath (updateProgressFn: Action<int>) =
        async {
            let request = WebRequest.Create(Uri(url))
            use! response = request.AsyncGetResponse()
            use stream = response.GetResponseStream()
            use writer = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write)

            let buffer = Array.zeroCreate 8192
            let rec copyTo () = 
                let read = stream.Read(buffer, 0, buffer.Length)
                if read > 0 then
                    writer.Write(buffer, 0, read)
                    updateProgressFn.Invoke read |> ignore
                    copyTo ()

            copyTo ()
            }

    member this.DownloadAsync(url, filePath, updateProgressFn: Action<int>) = 
        download url filePath updateProgressFn |> Async.StartAsTask
