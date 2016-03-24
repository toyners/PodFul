
namespace PodFul.Console
{
  using System;
  using PodFul.Library;

  class Program
  {
    static void Main(string[] args)
    {
      ChannelRecord record = ChannelFunctions.getChannelRecordFromRSS("RssURL", "Directory", @"C:\Projects\PodFul.Library\podcast.rss");

      Console.WriteLine(record.Title);
      Console.WriteLine(record.Website);
      Console.WriteLine(record.Description);
      Console.WriteLine(record.Podcasts.Length);
      Console.WriteLine();

      foreach (var podcastRecord in record.Podcasts)
      {
        Console.WriteLine(podcastRecord.Title);
        Console.WriteLine(podcastRecord.FileSize);
        Console.WriteLine(podcastRecord.URL);
        Console.WriteLine();
      }

      Console.ReadKey();
    }
  }
}
