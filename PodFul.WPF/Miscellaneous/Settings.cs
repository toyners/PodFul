﻿
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.IO;
  using System.Xml.Serialization;
  using FileDelivery;
  using Jabberwocky.Toolkit.Object;
  using PodFul.Library;

  public class Settings
  {
    #region Fields
    private String filePath;

    private SettingsData settingsData;
    #endregion

    #region Construction
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
            },

            new SettingsData.DeliveryPointData
            {
              Name = "Single",
              Location = @"C:\Users\toyne\Music\Podcasts\Daily"
            }
          }
        };
      }

      this.DeliveryPoints = this.CreateDeliveryPoints(this.settingsData.DeliveryData, fileDeliveryLogger);
    }
    #endregion

    #region Properties
    public UInt32 ConfirmPodcastDownloadThreshold
    {
      get { return this.settingsData.ConfirmPodcastDownloadThreshold; }
      set { this.settingsData.ConfirmPodcastDownloadThreshold = value; }
    }

    public UInt32 ConcurrentDownloadCount
    {
      get { return this.settingsData.ConcurrentDownloadCount; }
      set { this.settingsData.ConcurrentDownloadCount = value; }
    }

    public IDeliveryPoint[] DeliveryPoints { get; private set; }
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

    private IDeliveryPoint[] CreateDeliveryPoints(SettingsData.DeliveryPointData[] deliveryPointData, ILogger log)
    {
      var deliveryPoints = new IDeliveryPoint[deliveryPointData.Length];
      var index = 0;

      foreach (var data in deliveryPointData)
      {
        switch (data.Name)
        {
          case "Winamp": deliveryPoints[index] = new WinampDeliveryPoint(data.Location, log.Message, log.Exception);
          break;
          case "Single": deliveryPoints[index] = new FileDeliveryPoint(data.Location, log.Message, log.Exception);
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

      this.settingsData.DeliveryData = new SettingsData.DeliveryPointData[2];
      this.settingsData.DeliveryData[0].Name = "Winamp";
      this.settingsData.DeliveryData[0].Location = @"C:\Program Files (x86)\Winamp\winamp.exe";

      this.settingsData.DeliveryData[1].Name = "Single";
      this.settingsData.DeliveryData[1].Location = @"C:\Users\toyne\Music\Podcasts\Daily";
    }
    #endregion

    #region Structures
    public struct SettingsData
    {
      public UInt32 ConfirmPodcastDownloadThreshold;

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
