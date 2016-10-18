
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using Library;

  /// <summary>
  /// Interaction logic for DownloadConfirmation.xaml
  /// </summary>
  public partial class DownloadConfirmation : Window
  {
    public MessageBoxResult Result;

    public List<Int32> PodcastIndexes { get; private set; }

    public DownloadConfirmation(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      InitializeComponent();
    }

    private void Cancel(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.Cancel;
    }

    private void Skip_Click(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = null;
      this.Result = MessageBoxResult.No;
    }

    private void Download_Click(Object sender, RoutedEventArgs e)
    {
      this.PodcastIndexes = new List<Int32>();
      this.Result = MessageBoxResult.Yes;
    }
  }
}
