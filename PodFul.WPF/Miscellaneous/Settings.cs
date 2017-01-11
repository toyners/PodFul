
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Runtime.CompilerServices;
  using System.Xml.Serialization;
  using Jabberwocky.Toolkit.Object;

  public class Settings
  {
    #region Fields
    private String filePath;

    private SettingsData settingsData;
    #endregion

    #region Construction
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
          DeliveryData = new List<SettingsData.DeliveryPointData>()
        };
      }
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

    public List<SettingsData.DeliveryPointData> DeliveryPointData { get { return this.settingsData.DeliveryData; } }
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
      public UInt32 ConfirmPodcastDownloadThreshold { get; set; }

      public UInt32 ConcurrentDownloadCount { get; set; }

      public List<DeliveryPointData> DeliveryData { get; set; }
      #endregion

      #region Classes
      public class DeliveryPointData : NotifyPropertyChangedBase, INotifyPropertyChanged
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
            if (this.PropertyChanged != null)
            {
              PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Enabled"));
              PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TextColor"));
            }
          }
        }

        public String Location
        {
          get { return this.location; }
          set { this.SetField(ref this.location, value, this.PropertyChanged); }
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

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
      }
      #endregion
    }
    #endregion
  }
}
