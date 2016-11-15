
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Media;
  using Library;
  using Processing;

  /// <summary>
  /// Interaction logic for DownloadConfirmation.xaml
  /// </summary>
  public partial class DownloadConfirmation : Window
  {
    #region Fields
    private Boolean windowLoaded;

    private List<PodcastComparison> podcastComparisons;
    #endregion

    #region Construction
    public DownloadConfirmation(Feed oldFeed, Feed newFeed)
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

    private void SelectAllClick(Object sender, RoutedEventArgs e)
    {
      this.PodcastList.SelectedItems.Clear();

      for (Int32 i = 0; i < this.PodcastList.Items.Count; i++)
      {
        var item = this.PodcastList.Items[i];
        this.PodcastList.SelectedItems.Add(item);
      }

      this.DownloadButton.IsEnabled = true;

      DataGridRow row = this.PodcastList.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
      if (row != null)
      {
        DataGridCellsPresenter presenter = FindInVisualTree<DataGridCellsPresenter>(row);
        if (presenter != null)
        {
          DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
          if (cell != null)
          {
            cell.Focus();
          }
        }
      }
    }

    private static T FindInVisualTree<T>(DependencyObject obj) where T : DependencyObject
    {
      var count = VisualTreeHelper.GetChildrenCount(obj);
      for (Int32 i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);

        if (child is T)
        {
          return (T)child;
        }

        child = FindInVisualTree<T>(child);

        if (child != null)
        {
          return (T)child;
        }
      }

      return null;
    }

    private void SelectNewClick(Object sender, RoutedEventArgs e)
    {
      this.SelectNone();
      this.SelectNew();
    }

    private void SelectNew()
    {
      Int32 index = 0;
      while (index < this.podcastComparisons.Count && this.podcastComparisons[index].IsNewOnly)
      {
        var item = this.PodcastList.Items[index];
        this.PodcastList.SelectedItems.Add(item);

        DataGridRow row = this.PodcastList.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        if (row != null)
        {
          DataGridCellsPresenter presenter = FindInVisualTree<DataGridCellsPresenter>(row);
          if (presenter != null)
          {
            DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
            if (cell != null)
            {
              cell.Focus();
            }
          }
        }

        index++;
        this.DownloadButton.IsEnabled = true;
      }
    }

    private void SelectNoneClick(Object sender, RoutedEventArgs e)
    {
      this.SelectNone();
    }

    private void SelectNone()
    {
      this.PodcastList.UnselectAllCells();
      this.DownloadButton.IsEnabled = false;
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

    private void PodcastListSelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      DownloadButton.IsEnabled = (this.PodcastList.SelectedItems.Count > 0);
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
