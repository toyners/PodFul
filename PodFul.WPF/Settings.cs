
namespace PodFul.WPF
{
  using System;
  using PodFul.Library;

  public class Settings
  {
    #region Construction
    public Settings(ILogger log)
    {
      this.ConcurrentDownloadCount = 3;

      this.DeliveryPoints = this.CreateDeliveryPoints(log);
    }
    #endregion
    #region Properties
    public UInt32 ConcurrentDownloadCount { get; set; }

    public Action<Podcast, String>[] DeliveryPoints { get; private set; }
    #endregion

    #region Methods
    private Action<Podcast, String>[] CreateDeliveryPoints(ILogger log)
    {
      return new Action<Podcast, String>[]
      {
        new WinampDeliveryPoint(@"C:\Program Files (x86)\Winamp\winamp.exe", log.Message, log.Exception).DeliverToWinamp
      };
    }
    #endregion
  }
}
