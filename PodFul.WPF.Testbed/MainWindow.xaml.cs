
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Windows;
  using Miscellaneous;
  using PodFul.Library;
  using PodFul.WPF.Windows;
  using Processing;

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
      return CreateFeed(title, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now, podcasts);
    }

    private Feed CreateFeed(String title, String description, String website, String directory, String url, String imageFileName, DateTime creationDateTime, Podcast[] podcasts)
    {
      return new Feed(title, description, website, directory, url, String.Empty, imageFileName, podcasts, creationDateTime, DateTime.Now, true, true, true, 3);
    }

    private void FeedPropertiesWindow_Click(Object sender, RoutedEventArgs e)
    {
      var podcast = this.CreatePodcast("Podcast 1");
      var outputDirectory = System.IO.Directory.GetCurrentDirectory();
      var feed = this.CreateFeed("Test Feed", "Description for Test Feed", "Test Website", "Test Directory", "Test URL",
        System.IO.Path.Combine(outputDirectory, @"Question-Mark.jpg"),
        new DateTime(2018, 3, 3, 7, 46, 15), new[] { podcast }); 
      var window = new FeedPropertiesWindow(feed);
      window.ShowDialog();
    }

    private void RetryWindow_Click(Object sender, RoutedEventArgs e)
    {
      var podcast = this.CreatePodcast("Podcast 1");
      var feed = this.CreateFeed("Feed", new[] { podcast });
      FeedCollection feedCollection = null;
      DownloadJob[] jobs = { new DownloadJob(podcast, feed, feedCollection) };
      var window = new RetryWindow(jobs);
      window.ShowDialog();
    }
  }
}
