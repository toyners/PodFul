
namespace PodFul.Console
{
  using System;
  using PodFul.Library;

  class Program
  {
    static void Main(string[] args)
    {
      Channel channel = ChannelFunctions.readChannelRecordFromRSSFile("RssURL", "Directory", @"C:\Projects\PodFul\podcast.rss");

      DisplayChannel(channel);

      ChannelFunctions.writeChannelToFile(channel, @"C:\Projects\PodFul\output.txt");

      channel = ChannelFunctions.readChannelFromFile(@"C:\Projects\PodFul\output.txt");

      DisplayChannel(channel);

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
}
