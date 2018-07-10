
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using Library;

  public class PodcastViewModel : NotifyPropertyChangedBase
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource;
    private Int64 downloadedSize;
    private String failedMessage;
    private ProcessingStatus downloadState;
    private readonly FeedViewModel feedViewModel;
    private Boolean fileSizeKnown;
    private Int64 percentageStepSize;
    private readonly Podcast podcast;
    private String progressMajorSize;
    private String progressMinorSize;
    private String progressUnit;
    private Int32 progressValue;
    private Boolean useMarqueProgressStyle;
    #endregion

    #region Construction
    public PodcastViewModel(FeedViewModel feedViewModel, Podcast podcast, IFileDownloadProxyFactory fileDownProxyFactory)
    {
      this.feedViewModel = feedViewModel;
      this.podcast = podcast;
      this.DownloadState = ProcessingStatus.Idle;
    }
    #endregion

    #region Properties
    public String Description { get { return this.podcast.Description; } }
    public DateTime DownloadDate { get { return this.podcast.FileDetails.DownloadDate; } }
    public ProcessingStatus DownloadState
    {
      get { return this.downloadState; }
      private set { this.SetField(ref this.downloadState, value); }
    }
    public String FailedMessage
    {
      get { return this.failedMessage; }
      private set { this.SetField(ref this.failedMessage, value); }
    }
    public String FilePath { get { return this.podcast.FileDetails.FileName; } }
    public Int64 FileSize { get { return this.podcast.FileDetails.FileSize; } }
    public String PodcastImage { get { return this.podcast.FileDetails.ImageFileName; } }
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
    public DateTime PublishedDate { get { return this.podcast.PubDate; } }
    public String Title { get { return this.podcast.Title; } }
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
      if (this.cancellationTokenSource != null)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void IndividualDownload()
    {
      try
      {
        this.cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = this.cancellationTokenSource.Token;
        var fileDownloader = new FileDownloader();
        this.Download(fileDownloader, cancelToken, this.DownloadProgressEventHandler);
        this.cancellationTokenSource = null;
      }
      catch (OperationCanceledException)
      {
        // Download cancelled - nothing more to do
        this.DownloadState = ProcessingStatus.Idle;
      }
      catch (Exception e)
      {
        this.FailedMessage = e.Message;
        this.DownloadState = ProcessingStatus.Failed;
      }
    }

    public void ScanDownload(FileDownloader fileDownloader, CancellationToken cancelToken, Action<Int32> downloadProgressEventHandler)
    {
      this.Download(fileDownloader, cancelToken, downloadProgressEventHandler);
    }

    private void Download(FileDownloader fileDownloader, CancellationToken cancelToken, Action<Int32> downloadProgressEventHandler)
    {
      this.Initialise();

      var filePath = Path.Combine(this.feedViewModel.FeedDirectoryPath, this.podcast.FileDetails.FileName);
      fileDownloader.Download(this.podcast.URL, filePath, cancelToken, downloadProgressEventHandler);

      var fileInfo = new FileInfo(filePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", filePath));
      }

      this.podcast.SetFileDetails(fileInfo.Length, DateTime.Now);

      this.DownloadState = ProcessingStatus.Idle;
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
      if (this.downloadedSize < this.podcast.FileDetails.FileSize)
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

    private static void GetMajorMinorComponentsOfValue(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 decimalPointIndex = size.IndexOf('.');
      majorSize = size.Substring(0, decimalPointIndex);
      minorSize = size.Substring(decimalPointIndex);
    }

    private void Initialise()
    {
      this.DownloadState = ProcessingStatus.Downloading;
      this.downloadedSize = 0;
      this.percentageStepSize = this.podcast.FileDetails.FileSize / 100;
      this.ProgressMajorSize = "0";
      this.ProgressMinorSize = ".0";

      if (this.podcast.FileDetails.FileSize > 0)
      {
        this.fileSizeKnown = true;
        this.ProgressUnit = "%";
      }
      else
      {
        this.fileSizeKnown = false;
        this.ProgressUnit = " MB";
      }

      this.UseMarqueProgressStyle = !this.fileSizeKnown;
    }
    #endregion
  }
}
