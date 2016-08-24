
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Jabberwocky.Toolkit.String;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for SelectionWindow.xaml
  /// </summary>
  public partial class PodcastsWindow : Window
  {
    private enum SelectRowsType
    {
      None,
      All,
    }

    private String feedDirectory;

    #region Construction
    public PodcastsWindow(Feed feed)
    {
      InitializeComponent();

      var title = feed.Podcasts.Length + " podcast".Pluralize((UInt32)feed.Podcasts.Length);
      this.Title = title;

      this.PodcastList.ItemsSource = feed.Podcasts;
      this.feedDirectory = feed.Directory;

      this.PodcastList.Focus();
    }
    #endregion

    #region Properties
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

    private void Properties_Click(Object sender, RoutedEventArgs e)
    {
      var podcast = PodcastList.SelectedItem as Podcast;
      var propertiesWindow = new PodcastPropertiesWindow(podcast, this.feedDirectory);
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

    private void PodcastList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
      this.PodcastList_Scroller.ScrollToVerticalOffset(this.PodcastList_Scroller.VerticalOffset - e.Delta);
      e.Handled = true;
    }

    private void StartButton_Click(Object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
    #endregion
  }
}
