
namespace PodFul.WPF
{
  using System;
  using System.Windows;

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
      this.ConcurrentDownloadCount.Text = this.settings.ConcurrentDownloadCount.ToString();
    }
    #endregion

    #region Properties
    public DeliveryPointSetup[] GetDeliveryPointSetups
    {
      get
      {
        var winampDeliveryPoint = new WinampDeliveryPointSetup(@"C:\Program Files (x86)\Winamp\winamp.exe");
        return new[] { winampDeliveryPoint };
      }
    }
    #endregion

    #region Methods
    private void addButton_Click(Object sender, RoutedEventArgs e)
    {
      var addDeliveryPointWindow = new AddDeliveryPointWindow();
      addDeliveryPointWindow.ShowDialog();
    }

    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void editButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void removeButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void UpdateSettings()
    {
      try
      {
        this.settings.ConcurrentDownloadCount = UInt32.Parse(this.ConcurrentDownloadCount.Text);
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

  public abstract class DeliveryPointSetup
  { }

  public class WinampDeliveryPointSetup : DeliveryPointSetup
  {
    public WinampDeliveryPointSetup(String winampExePath)
    {
      this.WinampExePath = winampExePath;
    }

    public String WinampExePath { get; private set; }
  }
}
