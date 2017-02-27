
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;

  public interface ILogger
  {
    void Message(String message);

    void Message(String message, Boolean lineBreak);

    void Exception(String message);
  }

  public interface ILogManager
  {
    void Info(String key, String message);
  }

  public class LogController : ILogManager
  {
    private IDictionary<String, ILogger> loggers;
    public LogController(Dictionary<String, ILogger> loggers)
    {
      this.loggers = loggers;
    }

    public void Info(String key, String message)
    {
      throw new NotImplementedException();
    }
  }
}
