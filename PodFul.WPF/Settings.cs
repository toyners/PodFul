
namespace PodFul.WPF
{
  using System;
  using PodFul.Library;

  public class Settings
  {
    public Action<Podcast, String>[] CreateDeliveryPoints(ILogger log)
    {
      return new Action<Podcast, String>[]
      {
        new WinampDeliveryPoint(@"C:\Program Files (x86)\Winamp\winamp.exe", log.Message).DeliverToWinamp
      };
    }
  }
}
