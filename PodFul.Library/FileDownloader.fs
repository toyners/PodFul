namespace PodFul.Library

open System
open System.IO
open System.Net
open System.Threading

type FileDownloader() =

    let getResponseFromURL url : WebResponse = 
        // Downcast the web request object to a HttpWebRequest object so that 
        // the UserAgent property can be set.
        let uri = Uri(url)

        match (uri.Scheme) with
        | "http" -> 
            let request = WebRequest.Create(Uri(url)) :?> HttpWebRequest
            request.UserAgent <- "Podful Podcatcher";
            request.GetResponse()
        | _ ->
            WebRequest.Create(uri).GetResponse()

    let writeToFile filePath (cancelToken: CancellationToken) (updateProgressFn: Action<int>) (response : WebResponse) =
        
        use stream = response.GetResponseStream()
        use writer = new FileStream(filePath, FileMode.Create, FileAccess.Write)
        let buffer = Array.zeroCreate 8192
        let mutable continueLooping = true

        while continueLooping do

            cancelToken.ThrowIfCancellationRequested()

            let count = stream.Read(buffer, 0, buffer.Length)
            match (count) with
            | 0 -> continueLooping <- false
            | _ -> 
                writer.Write(buffer, 0, count)
                if updateProgressFn <> null then
                    updateProgressFn.Invoke count |> ignore

    let download (url: string) filePath (cancelToken: CancellationToken) (updateProgressFn: Action<int>) =
        getResponseFromURL url |> 
        writeToFile filePath cancelToken updateProgressFn

    member this.DownloadAsync(url, filePath,  cancelToken, updateProgressFn: Action<int>) = 
        async {
            download url filePath cancelToken updateProgressFn
        } |> Async.StartAsTask

    member this.Download(url, filePath, cancelToken, updateProgressFn: Action<int>) =
        download url filePath cancelToken updateProgressFn