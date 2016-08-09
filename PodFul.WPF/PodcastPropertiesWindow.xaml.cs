
namespace PodFul.WPF
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
    public PodcastPropertiesWindow(Podcast podcast, String feedDirectory)
    {
      InitializeComponent();

      this.Title = podcast.Title;

      this.Podcast = podcast;
      if (podcast.DownloadDate == FeedFunctions.NoDateTime)
      {
        this.FilePath = "No download";
      }
      else
      {
        this.FilePath = Path.Combine(feedDirectory, podcast.FileName);
      }

      this.DataContext = this; 
    }

    public Podcast Podcast { get; private set; }

    public String FilePath { get; private set; }

    private void CloseButton_Click(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}