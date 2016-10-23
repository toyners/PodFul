
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using Library;

  /// <summary>
  /// Interaction logic for DownloadConfirmation.xaml
  /// </summary>
  public partial class DownloadConfirmation : Window
  {
    public MessageBoxResult Result = MessageBoxResult.None;

    public List<Int32> PodcastIndexes { get; private set; }

    public DownloadConfirmation(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      InitializeComponent();

      var podcastComparison = new PodcastComparison();
      podcastComparison.OldTitle = oldFeed.Podcasts[0].Title;
      podcastComparison.NewTitle = newFeed.Podcasts[0].Title;

      var list = new List<PodcastComparison>();
      list.Add(podcastComparison);

      this.PodcastList.ItemsSource = list;
    }

    private void Cancel(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.Cancel;
      this.Close();
    }

    private void Skip_Click(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.No;
      this.Close();
    }

    private void Download_Click(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = new List<Int32>();
      this.Result = MessageBoxResult.Yes;
      this.Close();
    }

    private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.Result == MessageBoxResult.None)
      {
        this.Result = MessageBoxResult.Cancel;
        this.PodcastIndexes = null;
      }
    }
  }

  public class PodcastComparison
  {
    public String OldTitle { get; set; }

    public String NewTitle { get; set; }
  }
}
