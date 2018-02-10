
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
      var oldFeed = this.CreateFeed("OldFeed", new[] { this.CreatePodcast("Podcast B"), this.CreatePodcast("Podcast C") });
      var newFeed = this.CreateFeed("NewFeed", new[] { this.CreatePodcast("Podcast A"), this.CreatePodcast("Podcast C") });
      var window = new DownloadConfirmationWindow(oldFeed, newFeed);
      window.ShowDialog();
    }

    private Podcast CreatePodcast(String title)
    {
      var podcastFile = new PodcastFile(String.Empty, -1, DateTime.Now, String.Empty);
      return new Podcast(title, String.Empty, String.Empty, String.Empty, DateTime.Now, podcastFile);
    }

    private Feed CreateFeed(String title, Podcast[] podcasts)
    {
      return new Feed(title, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, podcasts, DateTime.Now, DateTime.Now, true, true, true);
    }
  }
}
