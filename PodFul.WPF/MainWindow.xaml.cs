
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Navigation;
  using System.Windows.Shapes;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private IFeedStorage feedStorage;
    private Feed currentFeed;
     
    public MainWindow()
    {
      InitializeComponent();

      this.DisplayTitle();

      var feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
      this.feedStorage = new FeedFileStorage(feedDirectory);
      this.feedStorage.Open();
      
      FeedList.ItemsSource = this.feedStorage.Feeds;

      if (this.feedStorage.Feeds.Length > 0)
      {
        this.currentFeed = this.feedStorage.Feeds[0];

        PodcastList.ItemsSource = this.currentFeed.Podcasts;
      }
    }

    private void DisplayTitle()
    {
      var guiVersion = Assembly.GetExecutingAssembly().GetName().Version;
      var libraryVersion = Assembly.GetAssembly(typeof(IFeedStorage)).GetName().Version;
      this.Title = String.Format("PodFul - v{0}.{1}.{2} (v{3}.{4}.{5})",
        guiVersion.Major,
        guiVersion.Minor,
        guiVersion.Build,
        libraryVersion.Major,
        libraryVersion.Minor,
        libraryVersion.Build);
    }

    private void addButton_Click(Object sender, RoutedEventArgs e)
    {
      var addFeedWindow = new AddFeedWindow();
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return;
      }

      // Create the feed and add to storage.
      var addFeed = addFeedWindow.DialogResult;

    }

    private void removeButton_Click(Object sender, RoutedEventArgs e)
    {
      var index = this.FeedList.SelectedIndex;

      this.feedStorage.Remove(this.currentFeed);
      this.FeedList.Items.RemoveAt(index);

      if (this.feedStorage.Feeds.Length == 0)
      {
        return;
      }

      if (index == 0)
      {
        this.FeedList.SelectedIndex = 0;
        return;
      }

      if (index == this.FeedList.Items.Count)
      {
        this.FeedList.SelectedIndex = this.FeedList.Items.Count - 1;
        return;
      }

      this.FeedList.SelectedIndex = index;
    }

    private void syncButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void scanButton_Click(Object sender, RoutedEventArgs e)
    {
      var title = String.Format("{0} feed{1}", this.feedStorage.Feeds.Length, (this.feedStorage.Feeds.Length != 1 ? "s" : String.Empty));
      var selectionWindow = new SelectionWindow(title);
      var startScanning = selectionWindow.ShowDialog();

      if (startScanning == null || !startScanning.Value)
      {
        return;
      }
    }

    private void downloadButton_Click(Object sender, RoutedEventArgs e)
    {
  
    }

    private void FeedList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      var index = (sender as ListBox).SelectedIndex;
      if (index == -1)
      {
        return;
      }

      var feed = this.feedStorage.Feeds[index];
      if (feed == this.currentFeed)
      {
        return;
      }

      this.currentFeed = feed;
      this.FeedDescription.Text = feed.Description;
    }
  }
}
