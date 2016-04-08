
namespace PodFul.Console
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using PodFul.Library;

  class Program
  {
    static void Main(string[] args)
    {
      var cancellationSource = new CancellationTokenSource();
      CancellationToken token = cancellationSource.Token;
      var test = new Test(cancellationSource);

      Task t = new File1().DownloadAsync(@"http://open.live.bbc.co.uk/mediaselector/5/redir/version/2.0/mediaset/audio-nondrm-download/proto/http/vpid/p03pmy3l.mp3",
        @"C:\Projects\PodFul\test.mp3",
        token,
        test.UpdateProgress);
      t.Wait();

      Console.ReadKey();
    }

    private static void DisplayChannel(Channel channel)
    {
      Console.WriteLine(channel.Title);
      Console.WriteLine(channel.Website);
      Console.WriteLine(channel.Description);
      Console.WriteLine(channel.Podcasts.Length);
      Console.WriteLine();

      if (channel.Podcasts.Length == 0)
      {
        return;
      }

      foreach (var podcastRecord in channel.Podcasts)
      {
        Console.WriteLine(podcastRecord.Title);
        Console.WriteLine(podcastRecord.FileSize);
        Console.WriteLine(podcastRecord.URL);
        Console.WriteLine();
      }
    }
  }

  public class Test
  {
    private CancellationTokenSource cancellationSource;
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
