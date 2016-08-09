
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
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

    #region Construction
    public PodcastsWindow(Podcast[] podcasts)
    {
      var title = String.Format("{0} podcast{1}", podcasts.Length, (podcasts.Length != 1 ? "s" : String.Empty));
      this.InitialiseWindow(title);

      this.PodcastList.ItemsSource = podcasts;
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
    private void InitialiseWindow(String title)
    {
      InitializeComponent();

      this.Title = title;

      this.PodcastList.Focus();
    }

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
      var propertiesWindow = new PodcastPropertiesWindow(podcast);
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
