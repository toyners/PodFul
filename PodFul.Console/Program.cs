
namespace PodFul.Console
{
  using System;
  using System.Threading.Tasks;
  using PodFul.Library;

  class Program
  {
    static void Main(string[] args)
    {
      /*Channel channel = ChannelFunctions.DownloadRSSFeed(@"C:\Projects\PodFul\podcast.rss");

      DisplayChannel(channel);

      ChannelFunctions.WriteChannelToFile(channel, @"C:\Projects\PodFul\output.txt");

      channel = ChannelFunctions.ReadChannelFromFile(@"C:\Projects\PodFul\output.txt");

      DisplayChannel(channel);*/
      var test = new Test();
      Task t = new File1().DownloadAsync(@"http://open.live.bbc.co.uk/mediaselector/5/redir/version/2.0/mediaset/audio-nondrm-download/proto/http/vpid/p03pmy3l.mp3",
        @"C:\Projects\PodFul\test.mp3", test.UpdateProgress);
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
    public void UpdateProgress(Int32 bytesRead)
    {
      Console.WriteLine(bytesRead);
    }
  }
}
