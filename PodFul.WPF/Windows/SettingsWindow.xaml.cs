﻿
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

    private void AddDirectoryDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      this.AddDeliveryPoint(Settings.SettingsData.DeliveryPointData.Types.Directory, "Add Directory Delivery Point");
    }

    private void AddWinampDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      this.AddDeliveryPoint(Settings.SettingsData.DeliveryPointData.Types.Winamp, "Add Winamp Delivery Point");
    }

    private void AddDeliveryPoint(Settings.SettingsData.DeliveryPointData.Types type, String title)
    {
      var addDeliveryPointWindow = new DeliveryPointWindow(type, title);
      addDeliveryPointWindow.Owner = this;

      var result = addDeliveryPointWindow.ShowDialog();

      if (!result.HasValue || !result.Value)
      {
        return;
      }

      var fullPath = addDeliveryPointWindow.FullPath.Text;
      this.deliveryPoints.Add(new Settings.SettingsData.DeliveryPointData { Type = type, Location = fullPath });
    }

    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void DeleteDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      var deliveryPointData = (Settings.SettingsData.DeliveryPointData)((Button)sender).DataContext;
      this.settings.DeliveryPointData.Remove(deliveryPointData);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.deliveryPoints.Remove(deliveryPointData);
      });
    }

    private void EditDeliveryPointClick(Object sender, RoutedEventArgs e)
    {
      var deliveryPointData = (Settings.SettingsData.DeliveryPointData)((Button)sender).DataContext;

      var title = "";
      if (deliveryPointData.Type == Settings.SettingsData.DeliveryPointData.Types.Winamp)
      {
        title = "Edit Winamp Delivery Point";
      }
      else
      {
        title = "Edit Directory Delivery Point";
      }

      this.EditDeliveryPoint(deliveryPointData, title);
    }

    private void EditDeliveryPoint(Settings.SettingsData.DeliveryPointData deliveryPointData, String title)
    {
      var editDeliveryPointWindow = new DeliveryPointWindow(deliveryPointData.Type, title, deliveryPointData.Location);
      editDeliveryPointWindow.Owner = this;

      var result = editDeliveryPointWindow.ShowDialog();

      if (!result.HasValue || !result.Value)
      {
        return;
      }

      deliveryPointData.Location = editDeliveryPointWindow.FullPath.Text;
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
