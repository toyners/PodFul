
using System.Windows;
using PodFul.WPF.Miscellaneous;
using PodFul.WPF.Windows;
using PodFul.WPF.Windows.TileView;

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

      Window mainWindow;
      if (settings.UseTileView)
      {
        mainWindow = new TileListWindow(settings, feedDirectory);
      }
      else
      {
        mainWindow = new MainWindow(settings, feedDirectory);
      }

      mainWindow.ShowDialog();
    }
  }
}
