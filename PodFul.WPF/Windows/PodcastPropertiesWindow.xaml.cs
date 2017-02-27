
namespace PodFul.WPF.Windows
{
  using System;
  using System.IO;
  using System.Windows;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for PodcastPropertiesWindow.xaml
  /// </summary>
  public partial class PodcastPropertiesWindow : Window
  {
    #region Construction
    public PodcastPropertiesWindow(Podcast podcast, String feedDirectory)
    {
      InitializeComponent();

      this.Title = podcast.Title;

      this.Podcast = podcast;
      if (podcast.FileDetails.DownloadDate == FeedFunctions.NoDateTime)
      {
        this.FilePath = "No download";
      }
      else
      {
        this.FilePath = Path.Combine(feedDirectory, podcast.FileName);
        if (!File.Exists(this.FilePath))
        {
          this.FilePath = "No file";
        }
      }

      this.FileSize = (new FileSizeConverter()).ConvertToString(podcast.FileDetails.FileSize) + "  (" + podcast.FileDetails.FileSize + " bytes)";

      this.DataContext = this;  
    }
    #endregion

    #region Properties
    public Podcast Podcast { get; private set; }

    public String FilePath { get; private set; }

    public String FileSize { get; private set; }
    #endregion

    #region Methods
    private void CloseButton_Click(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    #endregion
  }
}