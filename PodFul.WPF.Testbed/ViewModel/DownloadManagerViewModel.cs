
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.ComponentModel;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using PodFul.WPF.Testbed.Processing;

  public class DownloadManagerViewModel : NotifyPropertyChangedBase
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource;
    private Visibility cancellationVisibility = Visibility.Visible;
    private Int64 downloadedSize;
    private String exceptionMessage;
    private Boolean fileSizeKnown;
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
    private INewDownloadManager downloadManager;
    private String podcastTitle;
    private ProcessingStatus state = ProcessingStatus.Idle;
    #endregion

    #region Properties
    public CancellationToken CancellationToken { get; private set; }
    public Visibility CancellationVisibility
    {
      get { return this.cancellationVisibility; }
      set { this.SetField(ref this.cancellationVisibility, value); }
    }
    public String PodcastTitle
    {
      get { return this.podcastTitle; }
      private set { this.SetField(ref this.podcastTitle, value); }
    }
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

    public ProcessingStatus State
    {
      get { return this.state; }
      set { this.SetField(ref this.state, value); }
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
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void StartDownloading(INewDownloadManager downloadManager)
    {
      this.State = ProcessingStatus.Running;
      this.downloadManager = downloadManager;
      this.downloadManager.DownloadStartingEvent = this.InitialiseDownload;
      this.downloadManager.DownloadProgressEventHandler = this.DownloadProgressEventHandler;
      this.downloadManager.DownloadCompletedEvent += this.DownloadCompleted;
      this.downloadManager.DownloadPodcasts();
      this.State = ProcessingStatus.Completed;
    }

    public void Download()
    {
      try
      {
        //this.InitialiseDownload();

        var cancelToken = this.cancellationTokenSource.Token;

        this.Status = ProcessingStatus.Running;

        var fileDownloader = new FileDownloader();
        fileDownloader.Download(this.url, this.FilePath, cancelToken, this.DownloadProgressEventHandler);

        //this.DownloadCompleted();
      }
      catch (OperationCanceledException)
      {
        this.Status = ProcessingStatus.Cancelled;
        throw;
      }
      catch (Exception e)
      {
        this.ExceptionMessage = e.Message;
        this.Status = ProcessingStatus.Failed;
      }
    }

    public void DownloadCompleted(Podcast podcast)
    {
      if (this.fileSizeKnown)
      {
        // File size is known so set percentage to 100%
        this.ProgressMajorSize = "100";
        this.ProgressMinorSize = ".0";
      }

      this.ProgressValue = 0;
    }

    public void InitialiseDownload(Podcast podcast)
    {
      this.downloadedSize = 0;
      this.podcastSize = podcast.FileDetails.FileSize;
      this.percentageStepSize = this.podcastSize / 100;
      this.ProgressMajorSize = "0";
      this.ProgressMinorSize = ".0";
      this.PodcastTitle = podcast.Title;

      if (this.podcastSize > 0)
      {
        this.ProgressUnit = "%";
        this.fileSizeKnown = true;
      }
      else
      {
        this.ProgressUnit = " MB";
        this.fileSizeKnown = false;
      }

      this.UseMarqueProgressStyle = !this.fileSizeKnown;
    }

    private static void GetMajorMinorComponentsOfValue(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 decimalPointIndex = size.IndexOf('.');
      majorSize = size.Substring(0, decimalPointIndex);
      minorSize = size.Substring(decimalPointIndex);
    }

    private void DownloadProgressEventHandler(Int32 bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;

      String majorSize;
      String minorSize;
      if (!this.fileSizeKnown)
      {
        var downloadedSizeInMb = this.downloadedSize / 1048576.0;
        GetMajorMinorComponentsOfValue(downloadedSizeInMb, out majorSize, out minorSize);

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.ProgressMajorSize = majorSize;
          this.ProgressMinorSize = minorSize;
        });

        return;
      }

      Int64 value = 100;
      if (this.downloadedSize < this.podcastSize)
      {
        value = this.downloadedSize / this.percentageStepSize;
      }
      
      if (value < 100)
      {
        var percentageValue = (Double)this.downloadedSize / this.percentageStepSize;
        GetMajorMinorComponentsOfValue(percentageValue, out majorSize, out minorSize);
      }
      else
      { 
        majorSize = "100";
        minorSize = ".0";
      }

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressMajorSize = majorSize;
        this.ProgressMinorSize = minorSize;
        this.ProgressValue = (Int32)value;
      });
    }
    #endregion 
  }
}
