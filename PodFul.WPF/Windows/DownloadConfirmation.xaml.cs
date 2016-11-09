
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
    public MessageBoxResult Result = MessageBoxResult.None;

    public List<Int32> PodcastIndexes { get; private set; }
    #endregion

    #region Construction
    public DownloadConfirmation(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      InitializeComponent();

      var podcastComparisons = PodcastComparisonListCreator.Create(oldFeed.Podcasts, newFeed.Podcasts);
      this.PodcastList.ItemsSource = podcastComparisons;
    } 
    #endregion

    #region Methods
    private void CancelClick(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.Cancel;
      this.Close();
    }

    private void DownloadClick(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = new List<Int32>();
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

        DataGridRow row = this.PodcastList.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
        if (row != null)
        {
          DataGridCellsPresenter presenter = FindVisualTree<DataGridCellsPresenter>(row);
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
    }

    private static T FindVisualTree<T>(DependencyObject obj) where T : DependencyObject
    {
      var count = VisualTreeHelper.GetChildrenCount(obj);
      for (Int32 i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);

        if (child is T)
        {
          return (T)child;
        }
        else
        {
          child = FindVisualTree<T>(child);

          if (child != null)
          {
            return (T)child;
          }
        }
      }

      return null;
    }

    private void SelectNewClick(Object sender, RoutedEventArgs e)
    {
      
    }

    private void SelectNoneClick(Object sender, RoutedEventArgs e)
    {
      this.PodcastList.UnselectAllCells();
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
        this.Result = MessageBoxResult.Cancel;
        this.PodcastIndexes = null;
      }
    }
    #endregion
  }
}
