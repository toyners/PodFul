
using System.Windows;
using PodFul.WPF.Miscellaneous;
using PodFul.WPF.ViewModel;
using PodFul.WPF.Windows;

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup(System.Object sender, StartupEventArgs e)
    {
      var feedDirectory = WPF.Properties.Settings.Default.FeedDirectory;
      var settingsPath = WPF.Properties.Settings.Default.SettingsPath;
      var settings = new Settings(settingsPath);

      if (settings.UseTreeView)
      {
        var feedProcessor = new FeedProcessor();
        var feedCollectionViewModel = new FeedCollectionViewModel(feedProcessor);
        var mainWindow = new MainWindowNext(settings, feedCollectionViewModel);
        mainWindow.ShowDialog();
      }
      else
      {
        var mainWindow = new MainWindow(settings, feedDirectory);
        mainWindow.ShowDialog();
      }
    }
  }
}
