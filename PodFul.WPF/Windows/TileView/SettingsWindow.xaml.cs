
namespace PodFul.WPF.Windows.TileView
{
  using System;
  using System.Windows;
  using Miscellaneous;

  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : Window
  {
    #region Fields
    private Settings settings;
    #endregion

    #region Construction
    public SettingsWindow(Settings settings)
    {
      InitializeComponent();
      this.settings = settings;
      this.LoadUIFromSettings();
    }
    #endregion

    #region Methods
    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void LoadUIFromSettings()
    {
      this.ConcurrentScanCount.Text = this.settings.ConcurrentDownloadCount.ToString();
      this.DownloadImagesWhenAddingFeeds.IsChecked = this.settings.DownloadImagesWhenAddingFeeds;
      this.UseTileView.IsChecked = this.settings.UseTileView;
    }

    private void UpdateSettings()
    {
      try
      {
        this.settings.ConcurrentScanCount = UInt32.Parse(this.ConcurrentScanCount.Text);
        this.settings.DownloadImagesWhenAddingFeeds = (Boolean)this.DownloadImagesWhenAddingFeeds.IsChecked;
        this.settings.UseTileView = (Boolean)this.UseTileView.IsChecked;
        this.settings.Save();
      }
      catch (Exception exception)
      {
        var message = String.Format("Exception has occurred while saving settings to file. Message is:\r\n\r\n{0}", exception.Message);
        MessageBox.Show(message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      this.UpdateSettings();
    }
    #endregion
  }
}
