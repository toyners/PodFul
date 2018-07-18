
using System;
using System.Collections.Generic;
using System.Windows;
using PodFul.Library;
using PodFul.WPF.Logging;
using PodFul.WPF.Miscellaneous;
using PodFul.WPF.Processing.TileView;
using PodFul.WPF.ViewModel.TileView;
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
        var fileLogger = new FileLogger();
        var guiLogger = new UILogger();
        var combinedLogger = new CombinedLogger(fileLogger, guiLogger);
        var fileDeliveryLogger = new FileDeliveryLogger();
        var exceptionLogger = new FileLogger();

        var logController = new LogController(new Dictionary<String, ILogger>{
          { LoggerKeys.InfoKey, fileLogger },
          { LoggerKeys.ExceptionKey, exceptionLogger},
          { LoggerKeys.CombinedKey, combinedLogger },
          { LoggerKeys.UiKey, guiLogger}});
        var feedStorage = new JSONFileStorage(feedDirectory);
        var feedCollection = new FeedCollection(feedStorage);
        var fileDownloadProxyFactory = new FileDownloadProxyFactory();
        var feedCollectionViewModel = new TileListViewModel(feedCollection, fileDownloadProxyFactory);
        mainWindow = new TileListWindow(feedCollectionViewModel);
      }
      else
      {
        mainWindow = new MainWindow(settings, feedDirectory);
        mainWindow.ShowDialog();
      }

      mainWindow.ShowDialog();
    }
  }
}
