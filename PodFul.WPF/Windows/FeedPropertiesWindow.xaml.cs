
namespace PodFul.WPF.Windows
{
  using System;
  using System.Windows;
  using System.Windows.Forms;
  using Jabberwocky.Toolkit.Path;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for FeedPropertiesWindow.xaml
  /// </summary>
  public partial class FeedPropertiesWindow : Window
  {
    private Feed feed;

    #region Construction
    public FeedPropertiesWindow(Feed feed)
    {
      InitializeComponent();
      this.Title = feed.Title;
      this.DataContext = feed;
      this.feed = feed;
      this.DialogResult = false;
    }
    #endregion

    #region Methods
    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void ChangeDirectoryClick(Object sender, RoutedEventArgs e)
    {
      var folderBrowserDialog = new FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
      {
        return;
      }

      var selectedPath = PathOperations.CompleteDirectoryPath(folderBrowserDialog.SelectedPath);
      if (selectedPath != this.DirectoryPath.Text)
      {
        this.DirectoryPath.Text = selectedPath;
        this.DialogResult = true;
      }
    }
    #endregion
  }
}
