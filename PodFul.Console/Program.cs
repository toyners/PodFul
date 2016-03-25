
namespace PodFul.Console
{
  using System;
  using PodFul.Library;

  class Program
  {
    static void Main(string[] args)
    {
      ChannelRecord record = ChannelFunctions.readChannelRecordFromRSSFile("RssURL", "Directory", @"C:\Projects\PodFul\podcast.rss");

      DisplayChannel(record);

      ChannelFunctions.writeChannelRecordToFile(record, @"C:\Projects\PodFul\output.txt");

      record = ChannelFunctions.readChannelRecordFromFile(@"C:\Projects\PodFul\output.txt");

      DisplayChannel(record);

      Console.ReadKey();
    }

    private static void DisplayChannel(ChannelRecord record)
    {
      Console.WriteLine(record.Title);
      Console.WriteLine(record.Website);
      Console.WriteLine(record.Description);
      Console.WriteLine(record.Podcasts.Length);
      Console.WriteLine();

      if (record.Podcasts.Length == 0)
      {
        return;
      }

      foreach (var podcastRecord in record.Podcasts)
      {
        Console.WriteLine(podcastRecord.Title);
        Console.WriteLine(podcastRecord.FileSize);
        Console.WriteLine(podcastRecord.URL);
        Console.WriteLine();
      }
    }
  }
}
