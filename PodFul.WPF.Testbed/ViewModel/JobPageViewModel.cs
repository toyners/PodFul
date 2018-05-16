
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.WPF;

  public class JobPageViewModel : NotifyPropertyChangedBase
  {
    public JobPageViewModel(IList<JobViewModel> jobs, Int32 firstJobIndex, Int32 lastJobIndex)
    {
      this.Jobs = new List<JobViewModel>();
      while (firstJobIndex <= lastJobIndex)
      {
        this.Jobs.Add(jobs[firstJobIndex++]);
      }
    }

    public List<JobViewModel> Jobs { get; private set; }
  }
}
