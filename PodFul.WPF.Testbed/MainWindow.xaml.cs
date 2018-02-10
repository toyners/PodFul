
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Windows;
  using PodFul.Library;
  using PodFul.WPF.Windows;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Button_Click(Object sender, RoutedEventArgs e)
    {
      var podcastDetails = new PodcastFile(String.Empty, -1, DateTime.Now, String.Empty);
      var podcast = new Podcast("Test", String.Empty, String.Empty, String.Empty, DateTime.Now, podcastDetails);
      var oldFeed = new Feed("OldFeed", String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, new[] { podcast }, DateTime.Now, DateTime.Now, true, true, true);
      var window = new DownloadConfirmationWindow(oldFeed, oldFeed);
      window.ShowDialog();
    }
  }
}
