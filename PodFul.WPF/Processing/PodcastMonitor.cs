
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;

  public class PodcastMonitor
  {
    private CancellationTokenSource cancellationTokenSource;

    public String Name;

    public String ProgressMajorSize;

    public String ProgressMinorSize;

    public String ProgressUnit;

    public String FilePath;

    public String URL;

    public PodcastMonitor()
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.CancellationToken = this.cancellationTokenSource.Token;
    }

    public CancellationToken CancellationToken { get; private set; }

    public void CancelDownload()
    {
      if (this.CancellationToken.CanBeCanceled)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void ProgressEventHandler(int bytesRead)
    {

    }

    public void ExceptionEventHandler(Exception e)
    {

    }
  }
}
