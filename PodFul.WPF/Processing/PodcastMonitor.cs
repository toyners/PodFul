
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using System.Windows;
  using Library;

  public class PodcastMonitor : INotifyPropertyChanged
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource;

    private String message;

    private Int64 podcastSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;

    private String progressMajorSize;

    private String progressMinorSize;

    private Int32 progressValue;

    private Podcast podcast;
    #endregion

    #region Construction
    public PodcastMonitor(Podcast podcast, Int64 fileSize, String feedDirectory)
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.CancellationToken = this.cancellationTokenSource.Token;

      this.message = String.Empty;

      this.progressMajorSize = "0";
      this.progressMinorSize = ".0";

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

      this.percentageStepSize = this.podcastSize / 100;

      this.FilePath = Path.Combine(feedDirectory, podcast.FileName);

      this.podcast = podcast;
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties
    public CancellationToken CancellationToken { get; private set; }

    public String Message
    {
      get { return this.message; }
      set { this.SetField(ref this.message, value); }
    }

    public String FilePath { get; private set; }

    public Boolean FileSizeNotKnown { get; private set; }

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

    public String ProgressUnit { get; set; }

    public String URL { get { return this.podcast.URL; } }
    #endregion

    #region Methods
    public void CancelDownload()
    {
      if (this.CancellationToken.CanBeCanceled)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    internal void DownloadCompleted()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressValue = 0;
        this.Message = "Download Completed";
      });
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

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressMajorSize = majorSize;
        this.ProgressMinorSize = minorSize;

        if (this.FileSizeNotKnown)
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
      
      this.podcast.SetAllFileDetails(fileLength, DateTime.Now, imageResolver.GetName(this.podcast.FileDetails.ImageFileName));
    }

    private static void GetMajorMinorComponentsOfValue(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 index = size.IndexOf('.');
      majorSize = size.Substring(0, index);
      minorSize = size.Substring(index);
    }

    /// <summary>
    /// Set the field to the new value if it is different and then raises the property changed event handler.
    /// </summary>
    /// <typeparam name="T">Type of the field and value</typeparam>
    /// <param name="fieldValue">The existing field value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="propertyName">Name of the property being changed. Uses the name of the calling method by default.</param>
    private void SetField<T>(ref T fieldValue, T newValue, [CallerMemberName] String propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(fieldValue, newValue))
      {
        return;
      }

      fieldValue = newValue;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
