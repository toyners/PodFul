
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
    private Int64 downloadedSize;
    private FeedViewModel feedViewModel;
    private Boolean fileSizeKnown;
    private Int64 percentageStepSize;
    private Podcast podcast;
    private Int64 podcastSize;
    private String progressMajorSize;
    private String progressMinorSize;
    private String progressUnit;
    private Int32 progressValue;
    private Boolean useMarqueProgressStyle;
    #endregion

    #region Construction
    public PodcastViewModel(FeedViewModel feedViewModel, Podcast podcast)
    {
      this.feedViewModel = feedViewModel;
      this.podcast = podcast;
    }
    #endregion

    #region Properties
    public String Description { get { return this.podcast.Description; } }
    public DateTime DownloadDate { get { return this.podcast.FileDetails.DownloadDate; } }
    public String FilePath { get { return this.podcast.FileDetails.FileName; } }
    public Int64 FileSize { get { return this.podcast.FileDetails.FileSize; } }
    public String ImageFileName { get { return this.podcast.FileDetails.ImageFileName; } }
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
    public void Download(FileDownloader fileDownloader, CancellationToken cancelToken)
    {
      this.Initialise();

      var filePath = Path.Combine(this.feedViewModel.FeedDirectoryPath, this.podcast.FileDetails.FileName);
      fileDownloader.Download(this.podcast.URL, filePath, cancelToken, this.DownloadProgressEventHandler);

      var fileInfo = new FileInfo(filePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", filePath));
      }

      this.podcast.SetFileDetails(fileInfo.Length, DateTime.Now);
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

    private static void GetMajorMinorComponentsOfValue(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 decimalPointIndex = size.IndexOf('.');
      majorSize = size.Substring(0, decimalPointIndex);
      minorSize = size.Substring(decimalPointIndex);
    }

    private void Initialise()
    {
      this.podcastSize = this.podcast.FileDetails.FileSize;
      this.percentageStepSize = this.podcastSize / 100;
      this.ProgressMajorSize = "0";
      this.ProgressMinorSize = ".0";

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
    #endregion
  }
}
