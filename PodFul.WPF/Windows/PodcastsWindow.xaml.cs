
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Jabberwocky.Toolkit.String;
  using Jabberwocky.Toolkit.WPF;
  using Miscellaneous;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for PodcastsWindow.xaml
  /// </summary>
  public partial class PodcastsWindow : Window
  {
    #region Enums
    private enum SelectRowsType
    {
      None,
      All,
    }
    #endregion

    #region Fields
    private String feedDirectory;

    private ScrollViewer scroller;
    #endregion

    #region Construction
    public PodcastsWindow(Feed feed, Boolean deliverManualDownloads)
    {
      InitializeComponent();

      this.scroller = this.PodcastList.GetDescendantByType<ScrollViewer>();

      var title = feed.Podcasts.Length + " podcast".Pluralize((UInt32)feed.Podcasts.Length);
      this.Title = title;

      this.DeliverManualDownloads.IsChecked = deliverManualDownloads;

      this.PodcastList.ItemsSource = feed.Podcasts;
      this.feedDirectory = feed.Directory;

      this.PodcastList.Focus();
    }
    #endregion

    #region Properties
    public Boolean DeliverManualDownloadsToDeliveryPoints { get { return (Boolean)this.DeliverManualDownloads.IsChecked; } }

    public List<Int32> SelectedIndexes
    {
      get
      {
        var indexes = new List<Int32>();
        foreach (var row in this.PodcastList.SelectedItems)
        {
          indexes.Add(this.PodcastList.Items.IndexOf(row));
        }

        return indexes;
      }
    }
    #endregion

    #region Methods
    private void ClearButton_Click(Object sender, RoutedEventArgs e)
    {
      this.SelectRows(SelectRowsType.None);
    }

    private void AllButton_Click(Object sender, RoutedEventArgs e)
    {
      this.SelectRows(SelectRowsType.All);
    }

    private void Download_Click(Object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void PropertiesMenuItemClick(Object sender, RoutedEventArgs e)
    {
      var podcast = PodcastList.SelectedItem as Podcast;
      var podcastProperties = new PodFul.WPF.Miscellaneous.PodcastProperties(podcast, this.feedDirectory);   
      var propertiesWindow = new PodcastPropertiesWindow(podcastProperties);
      propertiesWindow.Owner = this;

      propertiesWindow.ShowDialog();
    }

    private void SelectRows(SelectRowsType selectRowsType)
    {
      if (selectRowsType == SelectRowsType.All)
      {
        this.PodcastList.SelectAll();
        return;
      }

      this.PodcastList.UnselectAll();
    }

    private void PodcastList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      this.startButton.IsEnabled = (sender as ListBox).SelectedItems.Count > 0;
    }

    private void PodcastList_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
    {
      this.DialogResult = true;
    }

    private void PodcastList_PreviewMouseDoubleClick(Object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType() == typeof(Border))
      {
        // The double click has happened on the border (e.g. scroll bar) of the list so ignore it.
        // Don't want this to trigger a unplaned action (i.e. window close)
        e.Handled = true;
      }
    }

    private void PodcastListMouseWheel(Object sender, MouseWheelEventArgs e)
    {
      var scrollValue = e.Delta < 0 ? 1 : -1;
      this.scroller.ScrollToVerticalOffset(this.scroller.VerticalOffset + scrollValue);
      e.Handled = true;
    }

    private void StartButton_Click(Object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
    #endregion
  }
}
