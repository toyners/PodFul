
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;

  public class JobPageNavigation : PageNavigation<JobPageViewModel, JobViewModel>
  {
    public Boolean HasJobs { get; private set; }

    public override void SetPages(IList<JobViewModel> items, Int32 itemCountPerPage, Func<IList<JobViewModel>, Int32, Int32, JobPageViewModel> instanceCreateFunction)
    {
      base.SetPages(items, itemCountPerPage, instanceCreateFunction);

      if (this.TotalPages > 0)
      {
        this.HasJobs = true;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("HasJobs"));
      }
    }
  }
}
