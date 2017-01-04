
namespace PodFul.WPF
{
  using System;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Controls;
  using Miscellaneous;

  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : Window
  {
    #region Fields
    private Settings settings;

    private ObservableCollection<Settings.SettingsData.DeliveryPointData> deliveryPoints;
    #endregion

    #region Construction
    public SettingsWindow(Settings settings)
    {
      InitializeComponent();
      this.settings = settings;
      this.ConcurrentDownloadCount.Text = this.settings.ConcurrentDownloadCount.ToString();
      this.ConfirmPodcastDownloadThreshold.Text = this.settings.ConfirmPodcastDownloadThreshold.ToString();

      this.deliveryPoints = new ObservableCollection<Settings.SettingsData.DeliveryPointData>(this.settings.DeliveryPointData);
      this.DeliveryPointList.ItemsSource = deliveryPoints;
    }
    #endregion

    #region Methods
    private void AddButtonClick(Object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      button.ContextMenu.IsEnabled = true;
      button.ContextMenu.PlacementTarget = button;
      button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
      button.ContextMenu.IsOpen = true;
    }

    private void AddSingleDeliveryPointClick(Object sender, RoutedEventArgs e)
    {

    }

    private void AddWinampDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      var addWinampDeliveryPointWindow = new WinampDeliveryPointWindow();
      addWinampDeliveryPointWindow.Title = "Add Winamp Delivery Point";
      addWinampDeliveryPointWindow.Owner = this;

      var result = addWinampDeliveryPointWindow.ShowDialog();

      if (!result.HasValue || !result.Value)
      {
        return;
      }

      this.deliveryPoints.Add(new Settings.SettingsData.DeliveryPointData { Name = "Winamp", Location = addWinampDeliveryPointWindow.FilePath.Text });
    }

    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void DeleteDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      var i = 0;
    }

    private void EditDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      var i = 0;
    }

    private void UpdateSettings()
    {
      try
      {
        this.settings.ConcurrentDownloadCount = UInt32.Parse(this.ConcurrentDownloadCount.Text);
        this.settings.ConfirmPodcastDownloadThreshold = UInt32.Parse(this.ConfirmPodcastDownloadThreshold.Text);
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
