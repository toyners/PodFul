
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;

  public class PodcastMonitor
  {
    public String Name;

    public String ProgressMajorSize;

    public String ProgressMinorSize;

    public String ProgressUnit;

    public String FilePath;

    public String URL;

    public CancellationToken CancellationToken;

    public void ProgressEventHandler(int bytesRead)
    {

    }

    public void ExceptionEventHandler(Exception e)
    {

    }
  }
}
