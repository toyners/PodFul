﻿
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Xml.Serialization;
  using Jabberwocky.Toolkit.Object;
  using Jabberwocky.Toolkit.WPF;

  public class Settings
  {
    #region Fields
    private String filePath;

    private SettingsData settingsData;
    #endregion

    #region Construction
    public Settings()
    {
      this.settingsData = new SettingsData
      {
        ConcurrentDownloadCount = 3,
        DeliverManualDownloads = false,
        DeliveryData = new List<SettingsData.DeliveryPointData>()
      };
    }

    public Settings(String filePath)
    {
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
        this.LoadFromFile();
      }
      else
      {
        this.ConfirmSettingsCanBeSaved();

        this.settingsData = new SettingsData
        {
          ConcurrentDownloadCount = 3,
          DeliverManualDownloads = false,
          DeliveryData = new List<SettingsData.DeliveryPointData>()
        };

        // Save the initial settings to file.
        this.Save();
      }
    }
    #endregion

    #region Properties
    public UInt32 ConcurrentDownloadCount
    {
      get { return this.settingsData.ConcurrentDownloadCount; }
      set { this.settingsData.ConcurrentDownloadCount = value; }
    }

    public UInt32 ConcurrentScanCount
    {
      get { return this.settingsData.ConcurrentScanCount; }
      set { this.settingsData.ConcurrentScanCount = value; }
    }

    public Boolean DeliverManualDownloadsToDeliveryPoints
    {
      get { return this.settingsData.DeliverManualDownloads; }
      set { this.settingsData.DeliverManualDownloads = value; }
    }

    public List<SettingsData.DeliveryPointData> DeliveryPointData { get { return this.settingsData.DeliveryData; } }

    public Boolean HideCompletedJobs
    {
      get { return this.settingsData.HideCompletedJobs; }
      set { this.settingsData.HideCompletedJobs = value; }
    }

    public Boolean DownloadImagesWhenAddingFeeds
    {
      get { return this.settingsData.DownloadImagesWhenAddingFeeds; }
      set { this.settingsData.DownloadImagesWhenAddingFeeds = value; }
    }

    public Boolean UseTileView
    {
      get { return this.settingsData.UseTileView; }
      set { this.settingsData.UseTileView = value; }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Saves the settings to the file path passed into the instance as part of construction. If
    /// no file path is available then the settings are not saved.
    /// </summary>
    public void Save()
    {
      if (this.filePath != null)
      {
        var serializer = new XmlSerializer(typeof(SettingsData));
        using (var fileStream = new FileStream(this.filePath, FileMode.Create))
        {
          serializer.Serialize(fileStream, this.settingsData);
        }
      }
    }

    private void ConfirmSettingsCanBeSaved()
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
    }

    private void LoadFromFile()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(SettingsData));
      using (var fileStream = new FileStream(this.filePath, FileMode.Open))
      {
        this.settingsData = (SettingsData)serializer.Deserialize(fileStream);
      }
    }
    #endregion

    #region Classes
    public class SettingsData
    {
      #region Properties
      public UInt32 ConcurrentDownloadCount { get; set; }

      public UInt32 ConcurrentScanCount { get; set; }

      public Boolean DeliverManualDownloads { get; set; }

      public List<DeliveryPointData> DeliveryData { get; set; }

      public Boolean HideCompletedJobs { get; set; }

      public Boolean DownloadImagesWhenAddingFeeds { get; set; }

      public Boolean UseTileView { get; set; }
      #endregion

      #region Classes
      public class DeliveryPointData : NotifyPropertyChangedBase
      {
        #region Enums
        public enum Types
        {
          Directory,
          Winamp
        }
        #endregion

        #region Fields
        private Boolean enabled;
        private String location;
        #endregion

        #region Properties
        public Boolean Enabled
        {
          get { return this.enabled; }
          set
          {
            this.enabled = value;
            this.TryInvokePropertyChanged(
              new PropertyChangedEventArgs("Enabled"),
              new PropertyChangedEventArgs("TextColor"));
          }
        }

        public String Location
        {
          get { return this.location; }
          set { this.SetField(ref this.location, value); }
        }

        public String TextColor
        {
          get
          {
            return (this.enabled ? "Black" : "Gray");
          }
        }

        public Types Type { get; set; }
        #endregion
      }
      #endregion
    }
    #endregion
  }
}
