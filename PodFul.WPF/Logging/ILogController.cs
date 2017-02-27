
namespace PodFul.WPF.Logging
{
  using System;

  public interface ILogController
  {
    void Exception(String key, String message);

    ILogger GetLogger(String key);

    void Info(String key, String message);
  }
}
