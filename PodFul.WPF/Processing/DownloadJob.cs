﻿
namespace PodFul.WPF.Processing
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using Miscellaneous;

  public class DownloadJob : NotifyPropertyChangedBase
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

    private Boolean fileSizeNotKnown;

    private Boolean useMarqueProgressStyle;

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
    public DownloadJob(Podcast podcast, Feed feed, FeedCollection feedCollection, IImageResolver imageResolver)
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

      this.imageResolver = imageResolver;
    }
    #endregion

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
        
        this.TryInvokePropertyChanged(
          new PropertyChangedEventArgs("StatusMessage"),
          new PropertyChangedEventArgs("StatusColor"),
          new PropertyChangedEventArgs("StatusWeight"));
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
          default: return "Blue";
        }
      }
    }

    public FontWeight StatusWeight
    {
      get
      {
        return (this.status != StatusTypes.Waiting ? FontWeights.Bold : FontWeights.Normal);
      }
    } 

    public String URL { get { return this.podcast.URL; } }

    public Boolean UseMarqueProgressStyle
    {
      get { return this.useMarqueProgressStyle; }
      set { this.SetField(ref this.useMarqueProgressStyle, value); }
    }
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

      this.feedCollection.UpdateFeed(this.feed);

      Application.Current.Dispatcher.Invoke(() =>
      {
        if (!this.fileSizeNotKnown)
        {
          // File size is known so set percentage to 100%
          this.ProgressMajorSize = "100";
          this.ProgressMinorSize = ".0";
        }

        this.ProgressValue = 0;
        this.Status = StatusTypes.Completed;
      });
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
          this.UseMarqueProgressStyle = this.fileSizeNotKnown = false;
        }
        else
        {
          this.ProgressUnit = " MB";
          this.UseMarqueProgressStyle = this.fileSizeNotKnown = true;
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
      if (this.fileSizeNotKnown)
      {
        var downloadedSizeInMb = this.downloadedSize / 1048576.0;
        GetMajorMinorComponentsOfValue(downloadedSizeInMb, out majorSize, out minorSize);
      }
      else
      {
        if (value >= 100)
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

        if (this.fileSizeNotKnown)
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
    #endregion
  }
}
