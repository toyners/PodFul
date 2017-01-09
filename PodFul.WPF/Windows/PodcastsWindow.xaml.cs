
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
      //var scroller = this.PodcastList.GetDescendantByType<ScrollViewer>();
      //scroller.ScrollToVerticalOffset(scroller.VerticalOffset - e.Delta);

      this.PodcastList_Scroller.ScrollToVerticalOffset(this.PodcastList_Scroller.VerticalOffset - e.Delta);
      e.Handled = true;
    }

    private void StartButton_Click(Object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
    #endregion
  }

  /*
   * https://dzone.com/articles/virtualization-wpf
   * http://stackoverflow.com/questions/10293236/accessing-the-scrollviewer-of-a-listbox-from-c-sharp
   * public static class Ext
  {
    public static T GetDescendantByType<T>(this System.Windows.Media.Visual element) where T : class
    {
      if (element == null)
      {
        return default(T);
      }

      if (element.GetType() == typeof(T))
      {
        return element as T;
      }
      T foundElement = null;
      if (element is FrameworkElement)
      {
        (element as FrameworkElement).ApplyTemplate();
      }
      for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(element); i++)
      {
        var visual = System.Windows.Media.VisualTreeHelper.GetChild(element, i) as System.Windows.Media.Visual;
        foundElement = visual.GetDescendantByType<T>();
        if (foundElement != null)
        {
          break;
        }
      }
      return foundElement;
    }
  }*/
}
