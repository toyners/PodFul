using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PodFul.WPF.Processing;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class RetryWindow : Window
  {
    public RetryWindow(IEnumerable<DownloadJob> jobs)
    {
      InitializeComponent();

      if (jobs == null || jobs.Count() == 0)
      {
        throw new ArgumentException("No jobs passed to RetryWindow cstr");
      }

      this.Jobs.ItemsSource = jobs;
    }

    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {

    }

    private void PodcastListMouseWheel(Object sender, MouseWheelEventArgs e)
    {

    }

    private void DownloadClick(Object sender, RoutedEventArgs e)
    {

    }

    private void CancelClick(Object sender, RoutedEventArgs e)
    {

    }

    private void SelectNoneClick(Object sender, RoutedEventArgs e)
    {

    }

    private void SelectAllClick(Object sender, RoutedEventArgs e)
    {

    }
  }
}
