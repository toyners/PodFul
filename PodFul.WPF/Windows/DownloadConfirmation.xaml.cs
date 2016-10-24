
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using Library;
  using Processing;

  /// <summary>
  /// Interaction logic for DownloadConfirmation.xaml
  /// </summary>
  public partial class DownloadConfirmation : Window
  {
    #region Fields
    public MessageBoxResult Result = MessageBoxResult.None;

    public List<Int32> PodcastIndexes { get; private set; }
    #endregion

    #region Construction
    public DownloadConfirmation(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      InitializeComponent();

      this.PodcastList.ItemsSource = this.CreatePodcastComparisonList(oldFeed.Podcasts, newFeed.Podcasts);
    }
    #endregion

    #region Methods
    private void Cancel(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.Cancel;
      this.Close();
    }

    private List<PodcastComparison> CreatePodcastComparisonList(Podcast[] oldPodcasts, Podcast[] newPodcasts)
    {
      var podcastComparisonLength = Math.Max(oldPodcasts.Length, newPodcasts.Length);
      var list = new List<PodcastComparison>(podcastComparisonLength);

      if (oldPodcasts.Length > newPodcasts.Length)
      {
        var oldIndex = 0;
        var oldCount = oldPodcasts.Length - newPodcasts.Length;

        while (oldCount-- > 0)
        {
          list.Add(new PodcastComparison(oldPodcasts[oldIndex++], null));
        }

        for (var newIndex = 0; newIndex < newPodcasts.Length; newIndex++)
        {
          list.Add(new PodcastComparison(oldPodcasts[oldIndex++], newPodcasts[newIndex]));
        }
      }
      else
      {
        var newIndex = 0;
        var newCount = newPodcasts.Length - oldPodcasts.Length;

        while (newCount-- > 0)
        {
          list.Add(new PodcastComparison(null, newPodcasts[newIndex++]));
        }

        for (var oldIndex = 0; oldIndex < oldPodcasts.Length; oldIndex++)
        {
          list.Add(new PodcastComparison(oldPodcasts[oldIndex], newPodcasts[newIndex++]));
        }
      }

      return list;
    }

    private void Download_Click(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = new List<Int32>();
      this.Result = MessageBoxResult.Yes;
      this.Close();
    }

    private void Skip_Click(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.No;
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
    #endregion
  }
}
