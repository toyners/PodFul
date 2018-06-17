
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.WPF;

  public class JobPageViewModel : NotifyPropertyChangedBase
  {
    public JobPageViewModel(IList<DownloadManagerViewModel> jobs, Int32 firstJobIndex, Int32 lastJobIndex)
    {
      this.Jobs = new List<DownloadManagerViewModel>();
      while (firstJobIndex <= lastJobIndex)
      {
        this.Jobs.Add(jobs[firstJobIndex++]);
      }
    }

    public List<DownloadManagerViewModel> Jobs { get; private set; }
  }
}
