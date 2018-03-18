using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PodFul.WPF.Processing;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class RetryWindow : Window
  {
    private ObservableCollection<RetryJob> retryJobs;
    private Int32 selectedCount;

    public RetryWindow(IEnumerable<DownloadJob> jobs)
    {
      InitializeComponent();

      if (jobs == null || jobs.Any())
      {
        throw new ArgumentException("No jobs passed to RetryWindow cstr");
      }

      this.retryJobs = new ObservableCollection<RetryJob>();
      foreach (var downloadJob in jobs)
      {
        this.retryJobs.Add(new RetryJob(downloadJob));
      }

      this.Jobs.ItemsSource = retryJobs;
    }

    private void CancelClick(Object sender, RoutedEventArgs e)
    {

    }

    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {

    }

    private void DownloadClick(Object sender, RoutedEventArgs e)
    {

    }

    private void PodcastListMouseWheel(Object sender, MouseWheelEventArgs e)
    {

    }

    private void SelectAllClick(Object sender, RoutedEventArgs e)
    {
      /*foreach (var job in this.retryJobs)
      {
        job.Retry = true;
      }*/

      for (Int32 i = 0; i < this.Jobs.Items.Count; i++)
      {
        var container = this.Jobs
                                 .ItemContainerGenerator
                                 .ContainerFromIndex(i);

        //var cb = container.GetDescendantByType<CheckBox>();
        var cb = FindVisualChild<CheckBox>(container);
        cb.IsChecked = true;
        //var template = container.;
        //var subPlotter = template.FindName("plotterCanal", container) as PlotterColetaCanalÚnico;
      }

      this.DownloadButton.IsEnabled = true;
      this.selectedCount = this.retryJobs.Count;
    }

    private childItem FindVisualChild<childItem>(DependencyObject obj)
                   where childItem : DependencyObject
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        if (child != null && child is childItem)
          return (childItem)child;
        else
        {
          childItem childOfChild = FindVisualChild<childItem>(child);
          if (childOfChild != null)
            return childOfChild;
        }
      }
      return null;
    }

    private void SelectNoneClick(Object sender, RoutedEventArgs e)
    {
      foreach (var job in this.retryJobs)
      {
        job.Retry = false;
      }

      this.DownloadButton.IsEnabled = false;
      this.selectedCount = 0;
    }

    private class RetryJob
    {
      public String Name { get; private set; }
      public String ExceptionMessage { get; private set; }
      public String Description { get; private set; }
      public Boolean Retry { get; set; }

      public RetryJob(DownloadJob job)
      {
        this.Name = job.Name;
        this.ExceptionMessage = job.ExceptionMessage;
        this.Description = job.Description;
        this.Retry = false;
      }
    }

    private void CheckedClick(Object sender, RoutedEventArgs e)
    {
      var item = (RetryJob)((FrameworkElement) sender).DataContext;
      this.selectedCount += (item.Retry ? 1 : -1);
      this.DownloadButton.IsEnabled = (this.selectedCount > 0);
    }
  }
}
