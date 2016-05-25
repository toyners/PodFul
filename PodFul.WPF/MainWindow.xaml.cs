
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

    }

    private void removeButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void syncButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void scanButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void downloadButton_Click(Object sender, RoutedEventArgs e)
    {

    }
  }
}
