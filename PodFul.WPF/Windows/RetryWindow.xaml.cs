using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Jabberwocky.Toolkit.WPF;
using PodFul.WPF.Processing;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class RetryWindow : Window
  {
    #region Fields
    private ObservableCollection<RetryJob> retryJobs;
    private CheckBox[] selectCheckBoxControls;
    private Int32 selectedCount;
    #endregion

    #region Construction
    public RetryWindow(IEnumerable<DownloadJob> jobs)
    {
      InitializeComponent();

      if (jobs == null || !jobs.Any())
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
    #endregion

    #region Properties
    public List<Int32> RetryJobIndexes { get; private set; }
    #endregion

    #region Methods
    private void Cancel()
    {
      this.RetryJobIndexes = null;
      this.DialogResult = false;
    }

    private void CancelClick(Object sender, RoutedEventArgs e)
    {
      this.Cancel();
    }

    private void CheckedClick(Object sender, RoutedEventArgs e)
    {
      var item = (RetryJob)((FrameworkElement)sender).DataContext;
      this.selectedCount += (item.Retry ? 1 : -1);
      this.DownloadButton.IsEnabled = (this.selectedCount > 0);
    }

    private void DownloadClick(Object sender, RoutedEventArgs e)
    {
      this.RetryJobIndexes = new List<Int32>(this.selectedCount);

      for(var i = 0; i < this.selectCheckBoxControls.Length; i++)
      {
        if (this.selectCheckBoxControls[i].IsChecked.GetValueOrDefault())
        {
          this.RetryJobIndexes.Add(i);
        }
      }

      this.DialogResult = true;
      this.Close();
    }

    private void SelectAllClick(Object sender, RoutedEventArgs e)
    {
      foreach (var selectCheckBoxControl in this.selectCheckBoxControls)
      {
        selectCheckBoxControl.IsChecked = true;
      }

      this.DownloadButton.IsEnabled = true;
      this.selectedCount = this.retryJobs.Count;
    }

    private void SelectNoneClick(Object sender, RoutedEventArgs e)
    {
      foreach (var selectCheckBoxControl in this.selectCheckBoxControls)
      {
        selectCheckBoxControl.IsChecked = true;
      }

      this.DownloadButton.IsEnabled = false;
      this.selectedCount = 0;
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      this.Cancel();
    }

    private void WindowLoaded(Object sender, RoutedEventArgs e)
    {
      this.selectCheckBoxControls = new CheckBox[this.retryJobs.Count];
      for (var i = 0; i < this.retryJobs.Count; i++)
      {
        var container = this.Jobs
                            .ItemContainerGenerator
                            .ContainerFromIndex(i);
        this.selectCheckBoxControls[i] = container.GetDescendantByType<CheckBox>();
      }
    }
    #endregion

    #region Classes
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
    #endregion
  }
}
