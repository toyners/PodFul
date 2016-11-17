
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using Library;
  using Miscellaneous;
  using Processing;

  /// <summary>
  /// Interaction logic for DownloadConfirmation.xaml
  /// </summary>
  public partial class DownloadConfirmationWindow : Window
  {
    #region Fields
    private Boolean windowLoaded;

    private List<PodcastComparison> podcastComparisons;
    #endregion

    #region Construction
    public DownloadConfirmationWindow(Feed oldFeed, Feed newFeed)
    {
      InitializeComponent();

      this.podcastComparisons = PodcastComparisonListCreator.Create(oldFeed.Podcasts, newFeed.Podcasts);
      this.PodcastList.ItemsSource = podcastComparisons;
      this.Result = MessageBoxResult.None;
    }
    #endregion

    #region Properties
    public MessageBoxResult Result { get; private set; }

    public List<Int32> PodcastIndexes { get; private set; }
    #endregion

    #region Methods
    private void CancelClick(Object sender, RoutedEventArgs e)
    {
      this.Cancel();
      this.Close();
    }

    private void Cancel()
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.Cancel;
    }

    private void DownloadClick(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = new List<Int32>(this.PodcastList.SelectedItems.Count);

      foreach (var selectedItem in this.PodcastList.SelectedItems)
      {
        this.PodcastIndexes.Add(this.PodcastList.Items.IndexOf(selectedItem));
      }

      this.Result = MessageBoxResult.Yes;
      this.Close();
    }

    private Int32 GetMatchingCount(Func<PodcastComparison, Boolean> matchFunc)
    {
      var index = 0;
      while (index < this.podcastComparisons.Count && matchFunc(this.podcastComparisons[index]))
      {
        index++;
      }

      return index;
    }

    private void PodcastListSelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      DownloadButton.IsEnabled = (this.PodcastList.SelectedItems.Count > 0);
    }

    private void Select(Func<PodcastComparison, Boolean> matchFunc)
    {
      var matchingCount = this.GetMatchingCount(matchFunc);

      if (matchingCount > 0)
      {
        this.PodcastList.SelectFirstRows(matchingCount);
        this.DownloadButton.IsEnabled = true;
      }
    }

    private void SelectAllClick(Object sender, RoutedEventArgs e)
    {
      this.SelectNone();
      this.Select((p) => { return p.HasNewPodcast; });
    }

    private void SelectNew()
    {
      this.Select((p) => { return p.HasNewPodcastOnly; });
    }

    private void SelectNewClick(Object sender, RoutedEventArgs e)
    {
      this.SelectNone();
      this.SelectNew();
    }

    private void SelectNone()
    {
      this.PodcastList.UnselectAllCells();
      this.DownloadButton.IsEnabled = false;
    }

    private void SelectNoneClick(Object sender, RoutedEventArgs e)
    {
      this.SelectNone();
    }

    private void SkipClick(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.No;
      this.Close();
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.Result == MessageBoxResult.None)
      {
        this.Cancel();
      }
    }

    private void WindowLoaded(Object sender, RoutedEventArgs e)
    {
      if (this.windowLoaded)
      {
        return;
      }

      this.windowLoaded = true;
      this.SelectNew();
    }
    #endregion
  }
}
