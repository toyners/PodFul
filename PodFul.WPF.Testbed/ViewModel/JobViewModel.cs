
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using Library;

  public class JobViewModel : NotifyPropertyChangedBase
  {
    private CancellationTokenSource cancellationTokenSource;
    private Visibility cancellationVisibility = Visibility.Visible;
    private Int64 downloadedSize;
    private String exceptionMessage;
    private Boolean fileSizeNotKnown;
    private Podcast podcast;
    private Int64 podcastSize;
    private Int64 percentageStepSize;
    private String progressMajorSize;
    private String progressMinorSize;
    private String progressUnit;
    private Int32 progressValue;
    private DownloadJobStatus status;
    private String url;

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
        this.status = DownloadJobStatus.Failed;
        return;
      }

      this.exceptionMessage = String.Empty;
      this.FilePath = Path.Combine(feed.Directory, this.podcast.FileDetails.FileName);
      this.status = DownloadJobStatus.Waiting;
    }

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
    public DownloadJobStatus LastStatus { get; private set; }
    public Int32 ProgressValue
    {
      get { return this.progressValue; }
      private set { this.SetField(ref this.progressValue, value); }
    }

    /*public Boolean UseMarqueProgressStyle { get { return false; } }
    public String StatusMessage { get { return ""; } }
    public String StatusColor { get { return ""; } }
    public String StatusWeight { get { return ""; } }*/

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

    public DownloadJobStatus Status
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

        switch (value)
        {
          case DownloadJobStatus.Failed:
          {
            this.CancellationVisibility = Visibility.Hidden;
            break;
          }

          case DownloadJobStatus.Cancelled:
          {
            this.CancellationVisibility = Visibility.Hidden;
            this.ExceptionMessage = String.Empty;
            break;
          }

          case DownloadJobStatus.Waiting:
          {
            this.ExceptionMessage = String.Empty;
            break;
          }
        }
      }
    }

    //public String ProgressUnit { get { return ""; } }

    public void Download()
    {
      var fileDownloader = new FileDownloader();
      fileDownloader.Download(this.url, this.FilePath, this.CancellationToken, this.ProgressEventHandler);

      var fileInfo = new FileInfo(this.FilePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", this.FilePath));
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
  }
}
