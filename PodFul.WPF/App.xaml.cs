﻿
using System;
using System.Collections.Generic;
using System.Windows;
using PodFul.Library;
using PodFul.WPF.Logging;
using PodFul.WPF.Miscellaneous;
using PodFul.WPF.Processing;
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
        var feedProcessor = new FeedProcessor(feedStorage, logController);
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
