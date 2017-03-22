
namespace PodFul.WPF.Windows
{
  using System;
  using System.Windows;
  using Miscellaneous;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for PodcastPropertiesWindow.xaml
  /// </summary>
  public partial class PodcastPropertiesWindow : Window
  {
    #region Construction
    public PodcastPropertiesWindow(PodcastProperties properties)
    {
      InitializeComponent();
      this.Title = properties.Title;
      this.DataContext = properties;  
    }
    #endregion

    #region Properties
    public Podcast Podcast { get; private set; }

    public String FilePath { get; private set; }

    public String FileSize { get; private set; }
    #endregion

    #region Methods
    private void CloseButton_Click(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
    #endregion
  }
}