
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.IO;
  using Library;

  public class PodcastViewModel
  {
    private FeedViewModel feedViewModel;
    private Podcast podcast;
    public PodcastViewModel(FeedViewModel feedViewModel, Podcast podcast)
    {
      this.feedViewModel = feedViewModel;
      this.podcast = podcast;
    }

    public String Description { get { return this.podcast.Description; } }
    public DateTime DownloadDate { get { return this.podcast.FileDetails.DownloadDate; } }
    public String FilePath { get { return this.podcast.FileDetails.FileName; } }
    public Int64 FileSize { get { return this.podcast.FileDetails.FileSize; } }
    public String ImageFileName { get { return this.podcast.FileDetails.ImageFileName; } }
    public DateTime PublishedDate { get { return this.podcast.PubDate; } }
    public String Title { get { return this.podcast.Title; } }
    public String URL { get { return this.podcast.URL; } }

    public void Download(FileDownloader fileDownloader, System.Threading.CancellationToken cancelToken, Action<Int32> downloadProgressEventHandler)
    {
      var filePath = Path.Combine(this.feedViewModel.FeedDirectoryPath, this.podcast.FileDetails.FileName);
      fileDownloader.Download(this.podcast.URL, filePath, cancelToken, downloadProgressEventHandler);

      var fileInfo = new FileInfo(filePath);
      if (!fileInfo.Exists)
      {
        throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", filePath));
      }

      this.podcast.SetFileDetails(fileInfo.Length, DateTime.Now);
    }
  }
}
