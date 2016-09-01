
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using System.Windows.Media;
  using Library;

  public class PodcastMonitor
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource;

    private Int64 podcastSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;

    private Podcast podcast;

    public String Name;

    public DateTime DownloadDate;

    public String ExceptionMessage;

    public Boolean FileSizeNotKnown;

    public String FilePath;

    public Int64 FileSize;

    public String ProgressMajorSize;

    public String ProgressMinorSize;

    public String ProgressUnit;

    public String URL;

    public String ImageFileName;
    #endregion

    #region Construction
    public PodcastMonitor(Podcast podcast, Int64 fileSize)
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.CancellationToken = this.cancellationTokenSource.Token;

      this.ProgressMajorSize = "0";
      this.ProgressMinorSize = ".0";

      this.podcastSize = fileSize;
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

      //this.ProgressBrush = Brush.;
      this.ExceptionMessage = String.Empty;
      this.percentageStepSize = this.podcastSize / 100;

      this.podcast = podcast;
    }
    #endregion

    #region Properties
    public CancellationToken CancellationToken { get; private set; }

    public Brush ProgressBrush { get; private set; }
    #endregion

    #region Methods
    public void CancelDownload()
    {
      if (this.CancellationToken.CanBeCanceled)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void DeliverPodcastFile(IFileDeliverer fileDeliver, String filePath)
    {
      fileDeliver?.Deliver(this.podcast, filePath);
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

      this.ProgressMajorSize = majorSize;
      this.ProgressMinorSize = minorSize;
    }

    public void SetPodcastFileDetails(IImageResolver imageResolver, Int64 fileLength)
    {
      this.podcast.SetAllFileDetails(fileLength, DateTime.Now, imageResolver?.GetName(this.podcast.FileDetails.ImageFileName));
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
