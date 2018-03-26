
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
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.DefaultExt = "mp3";
        saveFileDialog.Filter = "Audio File (*.mp3)|*.mp3";
        var continueDownload = saveFileDialog.ShowDialog(owner).GetValueOrDefault();
        if (continueDownload)
        {
          job.SetFilePath(saveFileDialog.FileName);
        }

        return continueDownload;
      };

      return jobNeedsLocationEventHandler;
    }
  }
}
