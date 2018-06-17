
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;

  public class JobPageNavigation : PageNavigation<JobPageViewModel, DownloadManagerViewModel>
  {
    public Boolean HasJobs { get; private set; }

    public override void SetPages(IList<DownloadManagerViewModel> items, Int32 itemCountPerPage = 2)
    {
      base.SetPages(items, itemCountPerPage);

      if (this.TotalPages > 0)
      {
        this.HasJobs = true;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("HasJobs"));
      }
    }

    public override void Reset()
    {
      base.Reset();
      this.HasJobs = false;
      this.TryInvokePropertyChanged(new PropertyChangedEventArgs("HasJobs"));
    }

    protected override JobPageViewModel CreatePage(IList<DownloadManagerViewModel> items, Int32 firstIndex, Int32 lastIndex)
    {
      return new JobPageViewModel(items, firstIndex, lastIndex);
    }
  }
}
