using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : Window
  {
    public SettingsWindow()
    {
      InitializeComponent();
    }

    private void addButton_Click(Object sender, RoutedEventArgs e)
    {
      var addDeliveryPointWindow = new AddDeliveryPointWindow();
      addDeliveryPointWindow.ShowDialog();
    }

    private void editButton_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void removeButton_Click(Object sender, RoutedEventArgs e)
    {

    }
  }
}
