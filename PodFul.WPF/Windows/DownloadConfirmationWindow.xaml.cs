﻿
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

    private Int32[] selectAllRowIndexes;

    private Int32 latestPodcastCount; // The count of latest podcasts found for this feed (no matches in the previous version of the feed)
    #endregion

    #region Construction
    public DownloadConfirmationWindow(Feed oldFeed, Feed newFeed)
    {
      InitializeComponent();

      this.podcastComparisons = PodcastComparisonListCreator.Create(oldFeed.Podcasts, newFeed.Podcasts);

      // Calculate the indexes for selecting all podcasts from the new feed and also
      // the first n podcasts to select when selecting the new podcasts only.
      var rowIndexes = new List<Int32>(this.podcastComparisons.Count);
      var newPodcastInFeed = true;
      for (var index = 0; index < this.podcastComparisons.Count; index++)
      {
        if (newPodcastInFeed && this.podcastComparisons[index].HasNewPodcastOnly)
        {
          this.latestPodcastCount++;
        }
        else
        {
          newPodcastInFeed = false;
        }

        if (this.podcastComparisons[index].HasNewPodcast)
        {
          rowIndexes.Add(index);
        }
      }

      this.selectAllRowIndexes = rowIndexes.ToArray();

      this.PodcastList.ItemsSource = podcastComparisons;
      this.Result = MessageBoxResult.None;
      this.Title = "Confirm Podcast Downloads for '" + newFeed.Title + "' feed.";
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
      this.PodcastIndexes = new List<Int32>();

      foreach (var selectedItem in this.PodcastList.SelectedItems)
      {
        var podcastComparisonIndex = this.PodcastList.Items.IndexOf(selectedItem);
        var podcastIndex = this.podcastComparisons[podcastComparisonIndex].Index;
        if (podcastIndex > -1)
        {
          this.PodcastIndexes.Add(podcastIndex);
        }
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

    private Boolean IsCtrlA(System.Windows.Input.KeyEventArgs e)
    {
      return e.Key == System.Windows.Input.Key.A &&
        (e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(System.Windows.Input.Key.RightCtrl));
    }

    private void PodcastListPreviewKeyDown(Object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (this.IsCtrlA(e))
      {
        e.Handled = true;
      }
    }

    private void PodcastListPreviewMouseRightButtonUp(Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      // No row selection for right mouse button clicks
      e.Handled = true;
    }

    private void PodcastListSelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      DownloadButton.IsEnabled = (this.PodcastList.SelectedItems.Count > 0);
    }

    private void SelectAll()
    {
      this.SelectNone();
      this.PodcastList.SelectSpecificRows(this.selectAllRowIndexes);
    }

    private void SelectAllClick(Object sender, RoutedEventArgs e)
    {
      this.SelectAll();
    }

    private void SelectNew()
    {
      this.SelectNone();
      this.PodcastList.SelectFirstRows(this.latestPodcastCount);
    }

    private void SelectNewClick(Object sender, RoutedEventArgs e)
    {
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
