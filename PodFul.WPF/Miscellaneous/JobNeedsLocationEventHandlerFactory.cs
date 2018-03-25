
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Windows;
  using Microsoft.Win32;
  using Processing;

  public static class JobNeedsLocationEventHandlerFactory
  {
    public static Func<DownloadJob, Boolean> CreateJobNeedsLocationEventHandler(Window owner)
    {
      Func<DownloadJob, Boolean> jobNeedsLocationEventHandler = job =>
      {
        var openFileDialog = new OpenFileDialog();
        var continueDownload = openFileDialog.ShowDialog(owner).GetValueOrDefault();
        if (continueDownload)
        {
          job.SetFilePath(openFileDialog.SafeFileName);
        }

        return continueDownload;
      };

      return jobNeedsLocationEventHandler;
    }
  }
}
