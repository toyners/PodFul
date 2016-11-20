
namespace PodFul.WPF
{
  using System;
  using System.Xml;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using PodFul.Library;

  public class Settings : IXmlSerializable
  {
    #region Construction
    public Settings(ILogger log)
    {
      this.ConcurrentDownloadCount = 3;

      this.ConfirmPodcastDownloadThreshold = 3;

      this.DeliveryPoints = this.CreateDeliveryPoints(log);
    }
    #endregion

    #region Properties
    public Int32 ConfirmPodcastDownloadThreshold { get; private set; }

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

    public XmlSchema GetSchema()
    {
      throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
