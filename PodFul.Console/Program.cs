
namespace PodFul.Console
{
  using System;
  using System.IO;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;
  using PodFul.Library;

  class Program
  {
    static void Main(string[] args)
    {
      //var url = @"http://open.live.bbc.co.uk/mediaselector/5/redir/version/2.0/mediaset/audio-nondrm-download/proto/http/vpid/p03pmy3l.mp3";
      //var url = @"http://www.giantbomb.com/podcasts/download/1563/Ep46_-_The_Giant_Beastcast-04-07-2016-4786568344.mp3";
      var url = @"http://www.bbc.co.uk/programmes/b006qykl/episodes/downloads.rss";

      // GetRSSFile(url);
      // DoWebRequest(url)

      var cancellationSource = new CancellationTokenSource();
      CancellationToken token = cancellationSource.Token;
      var test = new Test(cancellationSource);

      Task t = new FileDownloader().DownloadAsync(url,
        @"C:\Projects\PodFul\test.mp3",
        token,
        test.UpdateProgress);

      try
      {
        t.Wait();
      }
      catch (AggregateException ae)
      {
        ae = ae.Flatten();
        Console.WriteLine(ae.Message);
      }

      Console.ReadKey();
    }

    private static void GetRSSFile(String url)
    {
      var request = WebRequest.Create(url);
      var response = request.GetResponse();
      var stream = response.GetResponseStream();
      Byte[] buffer = new byte[2048];

      using (var writer = new FileStream(@"C:\Projects\file.rss", FileMode.Create, FileAccess.Write, FileShare.Read))
      {
        int read = 0;
        while ((read = stream.Read(buffer, 0, 2048)) != 0)
        {
          writer.Write(buffer, 0, read);
        }
      }
    }

    private static void DoWebRequest(String url)
    {
      try
      {
        var request = WebRequest.Create(url);
        ((HttpWebRequest)request).UserAgent = "Podful Podcatcher";
        var response = request.GetResponse();
      }
      catch (WebException we)
      {
        string responseText;

        using (var reader = new StreamReader(we.Response.GetResponseStream()))
        {
          responseText = reader.ReadToEnd();
        }
      }
    }

    private static void DisplayChannel(Feed feed)
    {
      Console.WriteLine(feed.Title);
      Console.WriteLine(feed.Website);
      Console.WriteLine(feed.Description);
      Console.WriteLine(feed.Podcasts.Length);
      Console.WriteLine();

      if (feed.Podcasts.Length == 0)
      {
        return;
      }

      foreach (var podcastRecord in feed.Podcasts)
      {
        Console.WriteLine(podcastRecord.Title);
        Console.WriteLine(podcastRecord.FileDetails.FileSize);
        Console.WriteLine(podcastRecord.URL);
        Console.WriteLine();
      }
    }
  }

  public class Test
  {
    private readonly CancellationTokenSource cancellationSource;
    private Int32 count = 5;

    public Test(CancellationTokenSource cancellationSource)
    {
      this.cancellationSource = cancellationSource;
    }

    public void UpdateProgress(Int32 bytesRead)
    {
      Console.WriteLine(bytesRead);
      if (--count == 0)
      {
        Console.WriteLine("Cancelled");
        this.cancellationSource.Cancel();
      }
    }
  }
}
