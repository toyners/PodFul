
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
      var oldFeed = this.CreateFeed("OldFeed", 
        new[] {
          this.CreatePodcast("Podcast B"),
          this.CreatePodcast("Podcast C"),
          this.CreatePodcast("Podcast E"),
          this.CreatePodcast("Podcast F"),
          this.CreatePodcast("Podcast H"),
          this.CreatePodcast("Podcast I")

        });

      var newFeed = this.CreateFeed("NewFeed",
        new[] {
          this.CreatePodcast("Podcast A"),
          this.CreatePodcast("Podcast C"),
          this.CreatePodcast("Podcast D"),
          this.CreatePodcast("Podcast F"),
          this.CreatePodcast("Podcast G"),
          this.CreatePodcast("Podcast I")
        });

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

    private Feed CreateFeed(String title, String description, String website, String directory, String url, String imageFileName, DateTime creationDateTime, Podcast[] podcasts)
    {
      return new Feed(title, description, website, directory, url, String.Empty, imageFileName, podcasts, creationDateTime, DateTime.Now, true, true, true);
    }

    private void FeedPropertiesWindow_Click(Object sender, RoutedEventArgs e)
    {
      var podcast = this.CreatePodcast("Podcast 1");
      var feed = this.CreateFeed("Test Feed", "Description for Test Feed", "Test Website", "Test Directory", "Test URL", 
        @"Question-Mark.jpg",
        new DateTime(2018, 3, 3, 7, 46, 15), new[] { podcast });
      var window = new FeedPropertiesWindow(feed);
      window.ShowDialog();
    }
  }
}
