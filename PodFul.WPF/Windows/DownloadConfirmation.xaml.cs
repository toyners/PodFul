
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

      this.PodcastList.ItemsSource = this.CreatePodcastComparisonList(oldFeed, newFeed);
    }
    #endregion

    #region Methods
    private void Cancel(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.Cancel;
      this.Close();
    }

    private List<PodcastComparison> CreatePodcastComparisonList(Feed oldFeed, Feed newFeed)
    {
      var list = new List<PodcastComparison>(newFeed.Podcasts.Length);
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
