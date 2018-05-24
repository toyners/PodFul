
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using Library;

  public class JobViewModel : NotifyPropertyChangedBase
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource;
    private Visibility cancellationVisibility = Visibility.Visible;
    private Int64 downloadedSize;
    private String exceptionMessage;
    private Boolean fileSizeNotKnown;
    private ProcessingStatus lastStatus = ProcessingStatus.Idle;
    private Podcast podcast;
    private Int64 podcastSize;
    private Int64 percentageStepSize;
    private String progressMajorSize;
    private String progressMinorSize;
    private String progressUnit;
    private Int32 progressValue;
    private ProcessingStatus status = ProcessingStatus.Waiting;
    private String url;
    private Boolean useMarqueProgressStyle;
    #endregion

    #region Construction
    public JobViewModel(Podcast podcast, Feed feed)
    {
      this.podcast = podcast;
      this.podcastSize = this.podcast.FileDetails.FileSize;
      this.progressMajorSize = this.progressMinorSize = this.progressUnit = String.Empty;
      this.percentageStepSize = this.podcastSize / 100;
      this.url = this.podcast.URL;

      var podcastSizeDescription = String.Empty;
      if (podcastSize <= 0)
      {
        podcastSizeDescription = "(unknown)";
      }
      else
      {
        podcastSizeDescription = Miscellaneous.GetReadableFileSize(podcastSize) + "Mb (" + podcastSize.ToString("#,##0") + " bytes)";
      }

      this.Description = "Feed: " + feed.Title + "\r\nSize: " + podcastSizeDescription;

      if (String.IsNullOrEmpty(this.podcast.FileDetails.FileName))
      {
        this.exceptionMessage = "No file name given.";
        this.status = ProcessingStatus.Failed;
        return;
      }

      this.exceptionMessage = String.Empty;
      this.FilePath = Path.Combine(feed.Directory, this.podcast.FileDetails.FileName);
      this.status = ProcessingStatus.Waiting;
    }
    #endregion

    #region Properties
    public CancellationToken CancellationToken { get; private set; }
    public Visibility CancellationVisibility
    {
      get { return this.cancellationVisibility; }
      set { this.SetField(ref this.cancellationVisibility, value); }
    }
    public String Title { get { return this.podcast.Title; } }
    public String Description { get; private set; }
    public String ExceptionMessage
    {
      get { return this.exceptionMessage; }
      set { this.SetField(ref this.exceptionMessage, value); }
    }
    public String FilePath { get; private set; }
    public ProcessingStatus LastStatus { get; private set; }
    public Int32 ProgressValue
    {
      get { return this.progressValue; }
      private set { this.SetField(ref this.progressValue, value); }
    }

    public String ProgressMajorSize
    {
      get { return this.progressMajorSize; }
      private set { this.SetField(ref this.progressMajorSize, value); }
    }

    public String ProgressMinorSize
    {
      get { return this.progressMinorSize; }
      private set { this.SetField(ref this.progressMinorSize, value); }
    }

    public String ProgressUnit
    {
      get { return this.progressUnit; }
      private set { this.SetField(ref this.progressUnit, value); }
    }

    public ProcessingStatus Status
    {
      get { return this.status; }
      set
      {
        this.LastStatus = this.status;
        this.status = value;

        this.TryInvokePropertyChanged(
          new PropertyChangedEventArgs("StatusMessage"),
          new PropertyChangedEventArgs("StatusColor"),
          new PropertyChangedEventArgs("StatusWeight"));

        switch (this.status)
        {
          case ProcessingStatus.Failed:
          {
            this.CancellationVisibility = Visibility.Hidden;
            break;
          }

          case ProcessingStatus.Cancelled:
          {
            this.CancellationVisibility = Visibility.Hidden;
            this.ExceptionMessage = String.Empty;
            break;
          }

          case ProcessingStatus.Waiting:
          {
            this.ExceptionMessage = String.Empty;
            break;
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
          case ProcessingStatus.Completed: return "Green";
          case ProcessingStatus.Cancelled: return "Orange";
          case ProcessingStatus.Failed: return "Red";
          case ProcessingStatus.Running: return "Black";
          default: return "Blue";
        }
      }
    }

    public String StatusMessage
    {
      get
      {
        switch (this.status)
        {
          case ProcessingStatus.Completed: return "Completed";
          case ProcessingStatus.Cancelled: return "Canceled";
          case ProcessingStatus.Failed: return "Failed";
          case ProcessingStatus.Running: return "Running";
          default: return "Waiting...";
        }
      }
    }

    public FontWeight StatusWeight
    {
      get
      {
        return (this.status != ProcessingStatus.Waiting ? FontWeights.Bold : FontWeights.Normal);
      }
    }

    public Boolean UseMarqueProgressStyle
    {
      get { return this.useMarqueProgressStyle; }
      set { this.SetField(ref this.useMarqueProgressStyle, value); }
    }
    #endregion

    #region Methods
    public void CancelDownload()
    {
      throw new NotImplementedException();
    }

    public void Download()
    {
      try
      {
        this.InitialiseDownload();

        var cancelToken = this.cancellationTokenSource.Token;

        this.Status = ProcessingStatus.Running;

        var fileDownloader = new FileDownloader();
        fileDownloader.Download(this.url, this.FilePath, cancelToken, this.ProgressEventHandler);

        this.DownloadCompleted();
      }
      catch (OperationCanceledException oce)
      {
        this.Status = ProcessingStatus.Cancelled;
      }
      catch (Exception e)
      {
        this.ExceptionMessage = e.Message;
        this.Status = ProcessingStatus.Failed;
      }
    }

    public void DownloadCompleted()
    {
      var fileInfo = new FileInfo(this.FilePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", this.FilePath));
      }

      /*if (this.imageResolver != null)
      {
        this.imageResolver.ResolvePodcastImage(this.podcast);
      }

      this.podcast.SetFileDetails(fileInfo.Length, DateTime.Now);
      this.feedCollection.UpdateFeedContent(this.feed);*/

      if (!this.fileSizeNotKnown)
      {
        // File size is known so set percentage to 100%
        this.ProgressMajorSize = "100";
        this.ProgressMinorSize = ".0";
      }

      this.ProgressValue = 0;
      this.CancellationVisibility = Visibility.Hidden;
      this.Status = ProcessingStatus.Completed;
    }

    public void InitialiseDownload()
    {
      this.cancellationTokenSource = new CancellationTokenSource();

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
    }

    private static void GetMajorMinorComponentsOfValue(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 decimalPointIndex = size.IndexOf('.');
      majorSize = size.Substring(0, decimalPointIndex);
      minorSize = size.Substring(decimalPointIndex);
    }

    private void ProgressEventHandler(Int32 bytesWrittenToFile)
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
    #endregion 
  }
}
