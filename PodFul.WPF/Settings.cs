
namespace PodFul.WPF
{
  using System;
  using System.IO;
  using System.Runtime.Serialization;
  using System.Xml;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using Newtonsoft.Json;
  using PodFul.Library;

  public class Settings : IXmlSerializable
  {
    #region Construction
    public static Settings Create(String filePath, ILogger fileDeliveryLogger)
    {
      /*var settingsPath = ConfigurationManager.AppSettings["SettingsPath"];
      XmlSerializer serializer = new XmlSerializer(typeof(Settings));
      FileStream stream = new FileStream(settingsPath, FileMode.Open);
      this.settings = (Settings)serializer.Deserialize(stream);*/
            
      //Settings settings = null;
      //settings.CreateDeliveryPoints(fileDeliveryLogger);
      var ctx = new StreamingContext(StreamingContextStates.Other, fileDeliveryLogger);
      var serializerSettings = new JsonSerializerSettings { Context = ctx };
      var fileStream = new FileStream(filePath, FileMode.Open);
      var sr = new StreamReader(filePath);

      var settings = JsonConvert.DeserializeObject<Settings>(sr.ReadToEnd(), serializerSettings);

      JsonConvert.SerializeObject(settings);

      throw new NotImplementedException();
    }

    [OnDeserializing]
    internal void OnDeserializing(StreamingContext ctx)
    {

    }

    public Settings(ILogger log)
    {
      this.ConcurrentDownloadCount = 3;

      this.ConfirmPodcastDownloadThreshold = 3;

      this.DeliveryPoints = this.CreateDeliveryPoints(log);
    }
    #endregion

    #region Properties
    public Int32 ConfirmPodcastDownloadThreshold { get; set; }

    public UInt32 ConcurrentDownloadCount { get; set; }

    public Action<Podcast, String>[] DeliveryPoints { get; private set; }
    #endregion

    #region Methods
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
