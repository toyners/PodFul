
namespace PodFul.WPF.Logging
{
  using System.Collections;

  public interface IFileDeliveryLogger : ILogger, IEnumerator, IEnumerable
  {
    void Clear();
  }
}
