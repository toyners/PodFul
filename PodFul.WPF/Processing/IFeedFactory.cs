
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using Library;

  public interface IFeedFactory
  {
    String FeedURL { get; }

    Feed Create(CancellationToken cancelToken);
  }
}
