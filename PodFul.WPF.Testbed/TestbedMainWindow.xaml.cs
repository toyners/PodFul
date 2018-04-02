
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Windows;
  using Jabberwocky.Toolkit.IO;
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

    private Podcast CreateTestPodcast(String title, String url, String fileName)
    {
      var podcastFile = new PodcastFile(fileName, -1, DateTime.Now, String.Empty);
      return new Podcast(title, String.Empty, url, String.Empty, DateTime.Now, podcastFile);
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
      var outputDirectory = Directory.GetCurrentDirectory();
      var feed = this.CreateTestFeed("Test Feed", "Description for Test Feed", "Test Website", "Test Directory", "Test URL",
        Path.Combine(outputDirectory, @"Question-Mark.jpg"),
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
      // Set up both feeds to have two new podcasts found during scanning
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath1 = Path.Combine(outputDirectory, testDirectoryName + " 1");
      var testURL1 = Path.Combine(outputDirectory, "Feed 1.rss");
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath1);
      var feed1 = this.CreateTestFeed("Test Feed 1", "Description for Test Feed 1", "Test Website 1", testDirectoryPath1, testURL1,
        null, DateTime.MinValue, 
        new Podcast[0]);

      // First feed has a download confirm threshold of 2 so the download confirmation window will be displayed.
      feed1 = Feed.SetConfirmDownloadThreshold(2, feed1);

      var testDirectoryPath2 = Path.Combine(outputDirectory, testDirectoryName + " 2");
      var testURL2 = Path.Combine(outputDirectory, "Feed 2.rss");
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath2);
      var feed2 = this.CreateTestFeed("Test Feed 2", "Description for Test Feed 2", "Test Website 2", testDirectoryPath2, testURL2,
        null, DateTime.MinValue,
        new Podcast[0]);

      // Second feed has a download confirmation of 3 so download confirmation window will NOT be displayed.
      feed2 = Feed.SetConfirmDownloadThreshold(3, feed2);

      var feeds = new[] { feed1, feed2 };

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
        new Queue<Int32>(new[] { 0, 1 }),
        null, // Image resolver not required for test
        null, // File delivery logger not required for test
        logController,
        downloadManager);

      var scanningWindow = new ScanningWindow((UInt32)feeds.Length, feedScanner, downloadManager, false);

      guiLogger.PostMessage = scanningWindow.PostMessage;

      scanningWindow.Owner = this;
      scanningWindow.ShowDialog();
    }

    private void RetryTestWithFailedRetry_Click(Object sender, RoutedEventArgs e)
    {
      // Set up feed to have two new podcasts found during scanning. One will fail with a bad url.
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath = Path.Combine(outputDirectory, testDirectoryName);
      var testURL = Path.Combine(outputDirectory, "Feed with Bad URLs.rss");
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath);
      var feed = this.CreateTestFeed("Test Feed", "Description for Test Feed", "Test Website", testDirectoryPath, testURL,
        null, DateTime.MinValue,
        new Podcast[0]);

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
        null, // Image resolver not required for test
        null, // File delivery logger not required for test
        logController,
        downloadManager);

      var scanningWindow = new ScanningWindow((UInt32)feeds.Length, feedScanner, downloadManager, false);

      guiLogger.PostMessage = scanningWindow.PostMessage;

      scanningWindow.Owner = this;
      scanningWindow.ShowDialog();

      var retryWindow = new RetryWindow(downloadManager.FailedJobs);
      retryWindow.ShowDialog();
      if (!retryWindow.DialogResult.GetValueOrDefault())
      {
        return;
      }

      var retryJobs = JobCollectionFactory.FilterJobsByIndex(downloadManager.FailedJobs, retryWindow.RetryJobIndexes);
      
      var retryManager = DownloadManager.Create(logController.GetLogger(MainWindow.CombinedKey), 1, null, null);
      var podcastDownloadWindow = new PodcastDownloadWindow(retryManager, false);

      // Add the jobs after creating the window so that job queued event will fire.
      retryManager.AddJobs(retryJobs);

      podcastDownloadWindow.Owner = this;
      podcastDownloadWindow.ShowDialog();
    }

    private void ManualDownloadWithFailingJob_Click(Object sender, RoutedEventArgs e)
    {
      // Create feed with two podcasts. One podcast has bad url. 
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath1 = Path.Combine(outputDirectory, testDirectoryName);
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath1);

      var podcast1 = this.CreateTestPodcast("Podcast 1", @"C:\Projects\PodFul\PodFul.WPF.Testbed\bin\Debug\Podcast1.mp3", Path.Combine(testDirectoryPath1 + "Podcast1.mp3"));
      var podcast2 = this.CreateTestPodcast("Podcast 2", "bad url", String.Empty);
      var feed = this.CreateTestFeed("Feed", new [] { podcast1, podcast2 });

      var feeds = new[] { feed };

      var feedStorage = Substitute.For<IFeedStorage>();
      feedStorage.Feeds.Returns(feeds);
      var feedCollection = new FeedCollection(feedStorage);

      var fileLogger = new FileLogger();
      var guiLogger = new UILogger();
      var combinedLogger = new CombinedLogger(fileLogger, guiLogger);

      var indexes = new List<Int32>(new [] { 0, 1 });
      var retryJobs = JobCollectionFactory.CreateJobsFromSelectedIndexesOfFeed(feed, indexes , feedCollection, null);

      var downloadManager = DownloadManager.Create(combinedLogger, 1, null, null);
      var podcastDownloadWindow = new PodcastDownloadWindow(downloadManager, false);

      // Add the jobs after creating the window so that job queued event will fire.
      downloadManager.AddJobs(retryJobs);

      podcastDownloadWindow.Owner = this;
      podcastDownloadWindow.ShowDialog();
    }

    private void ManualDownloadJob_Click(Object sender, RoutedEventArgs e)
    {
      // Create feed with two podcasts. One podcast has bad url. 
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath1 = Path.Combine(outputDirectory, testDirectoryName + " 1");
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath1);

      var podcast1 = this.CreateTestPodcast("Podcast 1", @"C:\Projects\PodFul\PodFul.WPF.Testbed\bin\Debug\Podcast1.mp3", Path.Combine(testDirectoryPath1 + "Podcast1.mp3"));
      var podcast2 = this.CreateTestPodcast("Podcast 2", @"C:\Projects\PodFul\PodFul.WPF.Testbed\bin\Debug\Podcast2.mp3", Path.Combine(testDirectoryPath1 + "Podcast2.mp3"));
      var feed1 = this.CreateTestFeed("Feed 1", new[] { podcast1, podcast2 });

      var feeds = new[] { feed1 };

      var feedStorage = Substitute.For<IFeedStorage>();
      feedStorage.Feeds.Returns(feeds);
      var feedCollection = new FeedCollection(feedStorage);

      var fileLogger = new FileLogger();
      var guiLogger = new UILogger();
      var combinedLogger = new CombinedLogger(fileLogger, guiLogger);

      var indexes = new List<Int32>(new[] { 0, 1 });
      var retryJobs = JobCollectionFactory.CreateJobsFromSelectedIndexesOfFeed(feed1, indexes, feedCollection, null);

      var downloadManager = DownloadManager.Create(combinedLogger, 1, null, null);
      var podcastDownloadWindow = new PodcastDownloadWindow(downloadManager, false);

      // Add the jobs after creating the window so that job queued event will fire.
      downloadManager.AddJobs(retryJobs);

      podcastDownloadWindow.Owner = this;
      podcastDownloadWindow.ShowDialog();
    }

    private void RetryTestWithSuccessfulRetry_Click(Object sender, RoutedEventArgs e)
    {
      // Set up feed to have two new podcasts found during scanning. One will fail with a bad url.
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath = Path.Combine(outputDirectory, testDirectoryName);
      var testURL = Path.Combine(outputDirectory, "Feed with Bad URLs.rss");
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath);
      var feed = this.CreateTestFeed("Test Feed", "Description for Test Feed", "Test Website", testDirectoryPath, testURL,
        null, DateTime.MinValue,
        new Podcast[0]);

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
        null, // Image resolver not required for test
        null, // File delivery logger not required for test
        logController,
        downloadManager);

      var scanningWindow = new ScanningWindow((UInt32)feeds.Length, feedScanner, downloadManager, false);

      guiLogger.PostMessage = scanningWindow.PostMessage;

      scanningWindow.Owner = this;
      scanningWindow.ShowDialog();

      var retryWindow = new RetryWindow(downloadManager.FailedJobs);
      retryWindow.ShowDialog();
      if (!retryWindow.DialogResult.GetValueOrDefault())
      {
        return;
      }

      // Insert a podcast with a good download path so that the retry job will work.
      var successfulPodcast = this.CreateTestPodcast(
        "Good Podcast",
        @"C:\Projects\PodFul\PodFul.WPF.Testbed\bin\Debug\Podcast1.mp3",
        Path.Combine(testDirectoryPath, "Podcast.mp3"));
      var successfulJob = new DownloadJob(successfulPodcast, feed, feedCollection);
      var retryJobs = new[] { successfulJob };

      var retryManager = DownloadManager.Create(logController.GetLogger(MainWindow.CombinedKey), 1, null, null);
      var podcastDownloadWindow = new PodcastDownloadWindow(retryManager, false);

      // Add the jobs after creating the window so that job queued event will fire.
      retryManager.AddJobs(retryJobs);

      podcastDownloadWindow.Owner = this;
      podcastDownloadWindow.ShowDialog();
    }

    private void ManualDownloadJobWithNoFilenameTest_Click(Object sender, RoutedEventArgs e)
    {
      // Set up feed to have two new podcasts found during scanning. One will fail with a bad url.
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath = Path.Combine(outputDirectory, testDirectoryName);
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath);

      var podcast = this.CreateTestPodcast(
        "Good Podcast",
        @"C:\Projects\PodFul\PodFul.WPF.Testbed\bin\Debug\Podcast1.mp3",
        String.Empty);
      var feed = this.CreateTestFeed("Test Feed", new[] { podcast });

      var feeds = new[] { feed };

      var feedStorage = Substitute.For<IFeedStorage>();
      feedStorage.Feeds.Returns(feeds);
      var feedCollection = new FeedCollection(feedStorage);

      var combinedLogger = new CombinedLogger(new FileLogger(), new UILogger());

      var jobs = JobCollectionFactory.CreateJobsFromSelectedIndexesOfFeed(feed, new[] { 0 }, feedCollection, null);

      var jobNeedsLocationEventHandler = JobNeedsLocationEventHandlerFactory.CreateJobNeedsLocationEventHandler(null);

      var downloadManager = DownloadManager.Create(combinedLogger, 1, jobNeedsLocationEventHandler);

      var podcastDownloadWindow = new PodcastDownloadWindow(downloadManager, false);

      // Add the jobs after creating the window so that job queued event will fire.
      downloadManager.AddJobs(jobs);

      podcastDownloadWindow.Owner = this;
      podcastDownloadWindow.ShowDialog();
    }

    private void ScanFeedWithNoPodcastFilenameTest_Click(Object sender, RoutedEventArgs e)
    {
      // Set up feed with one new podcast found during scanning. Podcast will have no URL (hence no filename)
      // and the job will throw a Name not found exception.
      var outputDirectory = Directory.GetCurrentDirectory();
      var testDirectoryName = "Test Directory";
      var testDirectoryPath = Path.Combine(outputDirectory, testDirectoryName);
      var testURL = Path.Combine(outputDirectory, "Feed with Missing URL.rss");
      DirectoryOperations.EnsureDirectoryIsEmpty(testDirectoryPath);
      var feed = this.CreateTestFeed("Test Feed", "Description for Test Feed", "Test Website", testDirectoryPath, testURL,
        null, DateTime.MinValue,
        new Podcast[0]);

      feed = Feed.SetConfirmDownloadThreshold(4, feed);

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
        null, // Image resolver not required for test
        null, // File delivery logger not required for test
        logController,
        downloadManager);

      var scanningWindow = new ScanningWindow((UInt32)feeds.Length, feedScanner, downloadManager, false);

      guiLogger.PostMessage = scanningWindow.PostMessage;

      scanningWindow.Owner = this;
      scanningWindow.ShowDialog();
    }

    private void MainViewNextVersion_Click(Object sender, RoutedEventArgs e)
    {
      var mainWindow = new MainWindowNext();
      mainWindow.Owner = this;
      mainWindow.ShowDialog();
    }
  }
}
