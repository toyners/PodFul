
namespace PodFul.WPF
{
  using System;
  using System.IO;
  using System.Xml.Serialization;
  using Jabberwocky.Toolkit.Object;
  using PodFul.Library;

  public class Settings
  {
    private String filePath;

    private SettingsData settingsData;

    #region Construction
    public static Settings Create(String filePath, ILogger fileDeliveryLogger)
    {
      /*var settingsPath = ConfigurationManager.AppSettings["SettingsPath"];
      XmlSerializer serializer = new XmlSerializer(typeof(Settings));
      FileStream stream = new FileStream(settingsPath, FileMode.Open);
      this.settings = (Settings)serializer.Deserialize(stream);*/
            
      //Settings settings = null;
      //settings.CreateDeliveryPoints(fileDeliveryLogger);
      /*var ctx = new StreamingContext(StreamingContextStates.Other, fileDeliveryLogger);
      var serializerSettings = new JsonSerializerSettings { Context = ctx };
      var fileStream = new FileStream(filePath, FileMode.Open);
      var sr = new StreamReader(filePath);

      var settings = JsonConvert.DeserializeObject<Settings>(sr.ReadToEnd(), serializerSettings);

      JsonConvert.SerializeObject(settings);*/

      throw new NotImplementedException();
    }

    public Settings(String filePath, ILogger fileDeliveryLogger)
    {
      fileDeliveryLogger.VerifyThatObjectIsNotNull("Parameter 'fileDeliveryLogger' is null.");
      filePath.VerifyThatObjectIsNotNull("Parameter 'filePath' is null.");
      if (filePath == String.Empty)
      {
        throw new ArgumentException("Parameter 'filePath' is empty.");
      }

      if (!Path.IsPathRooted(filePath))
      {
        throw new ArgumentException("Parameter 'filePath' contains a relative path.");
      }

      this.filePath = filePath;

      if (File.Exists(this.filePath))
      {
        this.LoadFromFile(fileDeliveryLogger);
      }
      else
      {
        try
        {
          var file = File.Create(this.filePath);
          file.Close();
        }
        catch (ArgumentException a)
        {
          if (a.Message == "Illegal characters in path.")
          {
            throw new ArgumentException("Parameter 'filePath' contains characters that are illegal in file paths.");
          }
        }

        this.settingsData = new SettingsData
        {
          ConcurrentDownloadCount = 3,
          ConfirmPodcastDownloadThreshold = 3,
          DeliveryData = new[]
          {
            new SettingsData.DeliveryPointData
            {
              Name = "Winamp",
              Location = @"C:\Program Files (x86)\Winamp\winamp.exe"
            }
          }
        };
      }

      this.DeliveryPoints = this.CreateDeliveryPoints(this.settingsData.DeliveryData, fileDeliveryLogger);
    }
    #endregion

    #region Properties
    public Int32 ConfirmPodcastDownloadThreshold
    {
      get { return this.settingsData.ConfirmPodcastDownloadThreshold; }
      set { this.settingsData.ConfirmPodcastDownloadThreshold = value; }
    }

    public UInt32 ConcurrentDownloadCount
    {
      get { return this.settingsData.ConcurrentDownloadCount; }
      set { this.settingsData.ConcurrentDownloadCount = value; }
    }

    public Action<Podcast, String>[] DeliveryPoints { get; private set; }
    #endregion

    #region Methods

    public void Save()
    {
      var serializer = new XmlSerializer(typeof(SettingsData));
      using (var fileStream = new FileStream(this.filePath, FileMode.Create))
      {
        serializer.Serialize(fileStream, this.settingsData);
      }
    }

    private Action<Podcast, String>[] CreateDeliveryPoints(SettingsData.DeliveryPointData[] deliveryPointData, ILogger log)
    {
      Action<Podcast, String>[] deliveryPoints = new Action<Podcast, String>[deliveryPointData.Length];
      var index = 0;

      foreach (var data in deliveryPointData)
      {
        switch (data.Name)
        {
          case "Winamp": deliveryPoints[index] = new WinampDeliveryPoint(data.Location, log.Message, log.Exception).DeliverToWinamp;
          break;
          default: throw new NotImplementedException("Delivery point '" + data.Name + "' not recognised");
        }

        index++;
      }

      return deliveryPoints;
    }

    private void LoadFromFile(ILogger fileDeliveryLogger)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
      using (var fileStream = new FileStream(this.filePath, FileMode.Open))
      {
        this.settingsData = (SettingsData)serializer.Deserialize(fileStream);
      }

      this.settingsData.DeliveryData = new SettingsData.DeliveryPointData[1];
      this.settingsData.DeliveryData[0].Name = "Winamp";
      this.settingsData.DeliveryData[0].Location = @"C:\Program Files (x86)\Winamp\winamp.exe";
    }
    #endregion

    #region Structures
    public struct SettingsData
    {
      public Int32 ConfirmPodcastDownloadThreshold;

      public UInt32 ConcurrentDownloadCount;

      public DeliveryPointData[] DeliveryData;

      public struct DeliveryPointData
      {
        public String Name;

        public String Location;
      }
    }
    #endregion
  }
}
