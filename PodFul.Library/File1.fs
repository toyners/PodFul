namespace PodFul.Library

open System
open System.IO
open System.Net
open System.Threading

type File1() =

    let download url filePath (cancelToken: CancellationToken) (updateProgressFn: Action<int>) =
        async {
            let request = WebRequest.Create(Uri(url))
            use! response = request.AsyncGetResponse()
            use stream = response.GetResponseStream()
            use writer = new FileStream(filePath, FileMode.Create, FileAccess.Write)

            let buffer = Array.zeroCreate 8192
            let rec copyTo () = 
                let read = stream.Read(buffer, 0, buffer.Length)
                if read > 0 && not (cancelToken.IsCancellationRequested) then
                    writer.Write(buffer, 0, read)
                    updateProgressFn.Invoke read |> ignore
                    copyTo ()

            copyTo ()
            }

    member this.DownloadAsync(url, filePath,  cancelToken, updateProgressFn: Action<int>) = 
        download url filePath cancelToken updateProgressFn |> Async.StartAsTask
