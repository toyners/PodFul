
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using Logging;
  using Miscellaneous;
  using NSubstitute;
  using PodFul.Library;
  using PodFul.WPF.Windows;
  using Processing;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class TestbedMainWindow : Window
  {
    public TestbedMainWindow()
    {
      InitializeComponent();
    }

    private void DownloadConfirmationWindow_Click(Object sender, RoutedEventArgs e)
    {
      var oldFeed = this.CreateTestFeed("OldFeed", 
        new[] {
          this.CreateTestPodcast("Podcast B"),
          this.CreateTestPodcast("Podcast C"),
          this.CreateTestPodcast("Podcast E"),
          this.CreateTestPodcast("Podcast F"),
          this.CreateTestPodcast("Podcast H"),
          this.CreateTestPodcast("Podcast I")

        });

      var newFeed = this.CreateTestFeed("NewFeed",
        new[] {
          this.CreateTestPodcast("Podcast A"),
          this.CreateTestPodcast("Podcast C"),
          this.CreateTestPodcast("Podcast D"),
          this.CreateTestPodcast("Podcast F"),
          this.CreateTestPodcast("Podcast G"),
          this.CreateTestPodcast("Podcast I")
        });

      var window = new DownloadConfirmationWindow(oldFeed, newFeed);
      window.ShowDialog();
    }

    private Podcast CreateTestPodcast(String title)
    {
      var podcastFile = new PodcastFile(String.Empty, -1, DateTime.Now, String.Empty);
      return new Podcast(title, String.Empty, String.Empty, String.Empty, DateTime.Now, podcastFile);
    }

    private Feed CreateTestFeed(String title, Podcast[] podcasts)
    {
      return CreateTestFeed(title, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, DateTime.Now, podcasts);
    }

    private Feed CreateTestFeed(String title, String description, String website, String directory, String url, String imageFileName, DateTime creationDateTime, Podcast[] podcasts)
    {
      return new Feed(title, description, website, directory, url, String.Empty, imageFileName, podcasts, creationDateTime, DateTime.Now, true, true, true, 3);
    }

    private void FeedPropertiesWindow_Click(Object sender, RoutedEventArgs e)
    {
      var podcast = this.CreateTestPodcast("Podcast 1");
      var outputDirectory = System.IO.Directory.GetCurrentDirectory();
      var feed = this.CreateTestFeed("Test Feed", "Description for Test Feed", "Test Website", "Test Directory", "Test URL",
        System.IO.Path.Combine(outputDirectory, @"Question-Mark.jpg"),
        new DateTime(2018, 3, 3, 7, 46, 15), new[] { podcast }); 
      var window = new FeedPropertiesWindow(feed);
      window.ShowDialog();
    }

    private void RetryWindow_Click(Object sender, RoutedEventArgs e)
    {
      var podcast1 = this.CreateTestPodcast("Podcast 1");
      var podcast2 = this.CreateTestPodcast("Podcast 2");
      var podcast3 = this.CreateTestPodcast("Podcast 3");
      var feed = this.CreateTestFeed("Feed", new[] { podcast1, podcast2, podcast3 });
      FeedCollection feedCollection = null;

      DownloadJob[] jobs = {
        new DownloadJob(podcast1, feed, feedCollection),
        new DownloadJob(podcast2, feed, feedCollection),
        new DownloadJob(podcast3, feed, feedCollection),
      };

      jobs[0].ExceptionMessage = "The Operation has timed out.";
      jobs[1].ExceptionMessage = "No filename given.";
      jobs[2].ExceptionMessage = "The Operation has timed out.";

      var window = new RetryWindow(jobs);
      window.ShowDialog();
    }

    private void ConfirmDownloadLimitTest_Click(Object sender, RoutedEventArgs e)
    {
      // Set old feed file

      var outputDirectory = System.IO.Directory.GetCurrentDirectory();
      var podcast = this.CreateTestPodcast("Podcast 1");
      var feed = this.CreateTestFeed("Test Feed", "Description for Test Feed", "Test Website", "", System.IO.Path.Combine(outputDirectory, "One Podcast.rss"),
        null, DateTime.MinValue, 
        new[] { podcast });

      var feeds = new[] { feed };

      var feedStorage = Substitute.For<IFeedStorage>();
      feedStorage.Feeds.Returns(feeds);
      var feedCollection = new FeedCollection(feedStorage);

      var fileLogger = new FileLogger();
      var guiLogger = new UILogger();
      var combinedLogger = new CombinedLogger(fileLogger, guiLogger);

      var logController = new LogController(new Dictionary<String, ILogger>{
          { MainWindow.InfoKey, fileLogger },
          { MainWindow.CombinedKey, combinedLogger },
          { MainWindow.UiKey, guiLogger}});

      var downloadManager = DownloadManager.Create(combinedLogger, 1, null);
      var feedScanner = new FeedScanner(
        feedCollection,
        new Queue<Int32>(new[] { 0 }),
        null, //Image resolver not required for test
        null,
        logController,
        downloadManager);

      var scanningWindow = new ScanningWindow(1u, feedScanner, downloadManager, false);
      scanningWindow.Owner = this;
      scanningWindow.ShowDialog();

      // Set new feed file
      // run scan 
    }
  }
}
