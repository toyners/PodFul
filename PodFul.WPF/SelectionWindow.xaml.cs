
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Controls;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for SelectionWindow.xaml
  /// </summary>
  public partial class SelectionWindow : Window
  {
    private enum SelectRowsType
    {
      None,
      All,
    }

    #region Construction
    public SelectionWindow(ObservableCollection<Feed> feeds)
    {
      var title = String.Format("{0} feed{1}", feeds.Count, (feeds.Count != 1 ? "s" : String.Empty));
      this.InitialiseWindow(title);

      this.ItemGrid.ItemsSource = feeds;
      this.ItemGrid.SelectAll();
    }

    public SelectionWindow(Podcast[] podcasts)
    {
      var title = String.Format("{0} podcast{1}", podcasts.Length, (podcasts.Length != 1 ? "s" : String.Empty));
      this.InitialiseWindow(title);

      this.ItemGrid.ItemsSource = podcasts;
    }
    #endregion

    #region Properties
    public List<Int32> SelectedIndexes
    {
      get
      {
        var indexes = new List<Int32>();
        foreach (var row in this.ItemGrid.SelectedItems)
        {
          indexes.Add(this.ItemGrid.Items.IndexOf(row));
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

      this.ItemGrid.Focus();
    }

    private void ClearButton_Click(Object sender, RoutedEventArgs e)
    {
      this.SelectRows(SelectRowsType.None);
    }

    private void AllButton_Click(Object sender, RoutedEventArgs e)
    {
      this.SelectRows(SelectRowsType.All);
    }

    private void SelectRows(SelectRowsType selectRowsType)
    {
      if (selectRowsType == SelectRowsType.All)
      {
        this.ItemGrid.SelectAll();
        return;
      }

      this.ItemGrid.UnselectAll();
    }
    
    private void ItemGrid_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      this.startButton.IsEnabled = (sender as DataGrid).SelectedItems.Count > 0;
    }

    private void startButton_Click(Object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
    #endregion
  }
}
