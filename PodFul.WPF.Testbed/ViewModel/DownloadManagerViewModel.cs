
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using PodFul.WPF.Testbed.Processing;

  public class DownloadManagerViewModel : NotifyPropertyChangedBase
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource;
    private Visibility cancellationVisibility = Visibility.Visible;
    private String downloadCount;
    private Int32 downloadNumber;
    private Int64 downloadedSize;
    private String exceptionMessage;
    private Boolean fileSizeKnown;
    private Int64 podcastSize;
    private Int64 percentageStepSize;
    private String progressMajorSize;
    private String progressMinorSize;
    private String progressUnit;
    private Int32 progressValue;
    private Boolean useMarqueProgressStyle;
    private INewDownloadManager downloadManager;
    private String podcastTitle;
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
    public String DownloadCount
    {
      get { return this.downloadCount; }
      private set { this.SetField(ref this.downloadCount, value); }
    }
    public String ExceptionMessage
    {
      get { return this.exceptionMessage; }
      set { this.SetField(ref this.exceptionMessage, value); }
    }
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
      this.downloadManager = downloadManager;
      this.downloadManager.DownloadStartingEvent = this.InitialiseDownload;
      this.downloadManager.DownloadProgressEventHandler = this.DownloadProgressEventHandler;
      this.downloadManager.DownloadCompletedEvent += this.DownloadCompleted;
      this.downloadManager.CompleteJobs();
    }

    public void DownloadCompleted()
    {
      if (this.fileSizeKnown)
      {
        // File size is known so set percentage to 100%
        this.ProgressMajorSize = "100";
        this.ProgressMinorSize = ".0";
      }

      this.ProgressValue = 0;
    }

    public void InitialiseDownload(PodcastViewModel podcastViewModel)
    {
      this.downloadedSize = 0;
      this.downloadNumber++;
      this.DownloadCount = $"[{this.downloadNumber}/{this.downloadManager.Count}]";
      this.podcastSize = podcastViewModel.FileSize;
      this.percentageStepSize = this.podcastSize / 100;
      this.ProgressMajorSize = "0";
      this.ProgressMinorSize = ".0";
      this.PodcastTitle = podcastViewModel.Title;

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
