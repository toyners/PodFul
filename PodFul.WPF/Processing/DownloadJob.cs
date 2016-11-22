
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using System.Windows;
  using Library;

  public class DownloadJob : INotifyPropertyChanged
  {
    public enum StatusTypes
    {
      Canceled,
      Completed,
      Failed,
      Running,
      Waiting
    }

    #region Fields
    private Boolean cancellationCanBeRequested = true;

    private Visibility cancellationVisibility = Visibility.Visible;

    private CancellationTokenSource cancellationTokenSource;

    private String exceptionMessage;

    private Feed feed;

    private FeedCollection feedCollection;

    private IFileDeliverer fileDeliverer;

    private Boolean fileSizeNotKnown;

    private IImageResolver imageResolver;

    private Int64 podcastSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;

    private String progressMajorSize;

    private String progressMinorSize;

    private String progressUnit;

    private Int32 progressValue;

    private Podcast podcast;

    private StatusTypes status;
    #endregion

    #region Construction
    public DownloadJob(Podcast podcast, Feed feed, FeedCollection feedCollection, IFileDeliverer fileDeliverer, IImageResolver imageResolver)
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.CancellationToken = this.cancellationTokenSource.Token;

      this.exceptionMessage = String.Empty;

      this.podcastSize = podcast.FileDetails.FileSize;

      this.progressMajorSize = this.progressMinorSize = this.progressUnit = String.Empty;
      
      this.percentageStepSize = this.podcastSize / 100;

      this.FilePath = Path.Combine(feed.Directory, podcast.FileName);

      this.status = StatusTypes.Waiting;

      this.podcast = podcast;

      this.feed = feed;

      this.feedCollection = feedCollection;

      this.fileDeliverer = fileDeliverer;

      this.imageResolver = imageResolver;
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    public Boolean CancellationCanBeRequested
    {
      get { return this.cancellationCanBeRequested; }
      set { this.SetField(ref this.cancellationCanBeRequested, value); }
    }

    public Visibility CancellationVisibility
    {
      get { return this.cancellationVisibility; }
      set { this.SetField(ref this.cancellationVisibility, value); }
    }

    public CancellationToken CancellationToken { get; private set; }

    public String ExceptionMessage
    {
      get { return this.exceptionMessage; }
      set { this.SetField(ref this.exceptionMessage, value); }
    }

    public String FilePath { get; private set; }

    public Boolean FileSizeNotKnown
    {
      get { return this.fileSizeNotKnown; }
      set { this.SetField(ref this.fileSizeNotKnown, value); }
    }

    public String Name { get { return this.podcast.Title; } }

    public String ProgressMajorSize
    {
      get { return this.progressMajorSize; }
      set { this.SetField(ref this.progressMajorSize, value); }
    }

    public String ProgressMinorSize
    {
      get { return this.progressMinorSize; }
      set { this.SetField(ref this.progressMinorSize, value); }
    }

    public Int32 ProgressValue
    {
      get { return this.progressValue; }
      set { this.SetField(ref this.progressValue, value); }
    }

    public String ProgressUnit
    {
      get { return this.progressUnit; }
      set { this.SetField(ref this.progressUnit, value); }
    }

    public String StatusMessage
    {
      get
      {
        switch (this.status)
        {
          case StatusTypes.Completed: return "Completed";
          case StatusTypes.Canceled: return "Canceled";
          case StatusTypes.Failed: return "Failed";
          case StatusTypes.Running: return "Running";
          default: return "Waiting...";
        }
      }
    }

    public StatusTypes Status
    {
      set
      {
        this.status = value;
        if (PropertyChanged != null)
        {
          // Change all status properties.
          PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StatusMessage"));
          PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StatusColor"));
          PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StatusWeight"));
          
          if (this.status == StatusTypes.Canceled)
          {
            this.CancellationCanBeRequested = false;
          }
        }
      }
    }

    public String StatusColor
    {
      get
      {
        switch (this.status)
        {
          case StatusTypes.Completed: return "Green";
          case StatusTypes.Canceled: return "Orange";
          case StatusTypes.Failed: return "Red";
          case StatusTypes.Running: return "Black";
          default: return "DarkSlateGray";
        }
      }
    }

    public FontWeight StatusWeight
    {
      get
      {
        switch (this.status)
        {
          case StatusTypes.Completed: return FontWeights.Bold;
          case StatusTypes.Canceled: return FontWeights.Bold;
          case StatusTypes.Failed: return FontWeights.Bold;
          case StatusTypes.Running: return FontWeights.Normal;
          default: return FontWeights.Normal;
        }
      }
    } 

    public String URL { get { return this.podcast.URL; } }
    #endregion

    #region Methods
    public void CancelDownload()
    {
      if (this.CancellationToken.CanBeCanceled)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void DownloadCanceled()
    {
      if (File.Exists(this.FilePath))
      {
        File.Delete(this.FilePath);
      }

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressValue = 0;
        this.ProgressMajorSize = this.ProgressMinorSize = this.ProgressUnit = String.Empty;
        this.CancellationVisibility = Visibility.Hidden;
        this.Status = StatusTypes.Canceled;
      });
    }

    public void DownloadCompleted()
    {
      var fileInfo = new FileInfo(this.FilePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", this.FilePath));
      }

      this.SetPodcastFileDetails(this.imageResolver, fileInfo.Length);
      if (this.fileDeliverer != null)
      {
        this.DeliverPodcastFile(this.fileDeliverer, this.FilePath);
      }

      this.feedCollection.UpdateFeed(this.feed);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressValue = 0;
        this.Status = StatusTypes.Completed;
      });
    }

    public void DeliverPodcastFile(IFileDeliverer fileDeliver, String filePath)
    {
      fileDeliver?.Deliver(this.podcast, filePath);
    }

    public void InitialiseBeforeDownload()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressMajorSize = "0";
        this.ProgressMinorSize = ".0";

        if (this.podcastSize > 0)
        {
          this.ProgressUnit = "%";
          this.FileSizeNotKnown = false;
        }
        else
        {
          this.ProgressUnit = "MB";
          this.FileSizeNotKnown = true;
        }

        this.Status = StatusTypes.Running;
      });
    }

    public void ProgressEventHandler(int bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;

      Int64 value = 100;
      if (this.downloadedSize < this.podcastSize)
      {
        value = this.downloadedSize / this.percentageStepSize;
      }

      String majorSize;
      String minorSize;
      if (this.FileSizeNotKnown)
      {
        var downloadedSizeInMb = this.downloadedSize / 1048576.0;
        GetMajorMinorComponentsOfValue(downloadedSizeInMb, out majorSize, out minorSize);
      }
      else
      {
        if (value == 100)
        {
          majorSize = "100";
          minorSize = ".0";
        }
        else
        {
          var percentageValue = (Double)this.downloadedSize / this.percentageStepSize;
          GetMajorMinorComponentsOfValue(percentageValue, out majorSize, out minorSize);
        }
      }

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressMajorSize = majorSize;
        this.ProgressMinorSize = minorSize;

        if (this.FileSizeNotKnown)
        {
          return;
        }

        this.ProgressValue = (Int32)value;
      });
    }

    public void SetPodcastFileDetails(IImageResolver imageResolver, Int64 fileLength)
    {
      if (imageResolver == null)
      {
        throw new Exception("Should not get here");
      }

      var imagePath = imageResolver.GetName(this.podcast.FileDetails.ImageFileName, this.podcast.ImageURL);
      this.podcast.SetAllFileDetails(fileLength, DateTime.Now, imagePath);
    }

    private static void GetMajorMinorComponentsOfValue(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 index = size.IndexOf('.');
      majorSize = size.Substring(0, index);
      minorSize = size.Substring(index);
    }

    /// <summary>
    /// Set the field to the new value if it is different and then raises the property changed event handler.
    /// </summary>
    /// <typeparam name="T">Type of the field and value</typeparam>
    /// <param name="fieldValue">The existing field value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="propertyName">Name of the property being changed. Uses the name of the calling method by default.</param>
    private void SetField<T>(ref T fieldValue, T newValue, [CallerMemberName] String propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(fieldValue, newValue))
      {
        return;
      }

      fieldValue = newValue;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
