
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using static Processing.DownloadJob;

  public class JobViewModel : NotifyPropertyChangedBase
  {
    private String exceptionMessage;
    private Podcast podcast;
    private Int64 podcastSize;
    private Int64 percentageStepSize;
    private String progressMajorSize;
    private String progressMinorSize;
    private String progressUnit;
    private Int32 progressValue;
    private StatusTypes status;

    public JobViewModel(Podcast podcast, Feed feed)
    {
      this.podcast = podcast;
      this.podcastSize = this.podcast.FileDetails.FileSize;
      this.progressMajorSize = this.progressMinorSize = this.progressUnit = String.Empty;
      this.percentageStepSize = this.podcastSize / 100;

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
        this.status = StatusTypes.Failed;
        return;
      }

      this.exceptionMessage = String.Empty;
      this.FilePath = Path.Combine(feed.Directory, this.podcast.FileDetails.FileName);
      this.status = StatusTypes.Waiting;
    }

    public String Title { get { return this.podcast.Title; } }
    public String Description { get; private set; }
    public String FilePath { get; private set; }
    public Int32 ProgressValue
    {
      get { return this.progressValue; }
      set
      {
        this.SetField(ref this.progressValue, value);
      }
    }
    /*public Boolean UseMarqueProgressStyle { get { return false; } }
    public String StatusMessage { get { return ""; } }
    public String StatusColor { get { return ""; } }
    public String StatusWeight { get { return ""; } }
    public String ExceptionMessage { get { return ""; } }

    public String ProgressMajorSize { get { return ""; } }
    public String ProgressMinorSize { get { return ""; } }
    public String ProgressUnit { get { return ""; } }*/
  }
}
