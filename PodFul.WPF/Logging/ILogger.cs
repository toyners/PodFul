
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

  public interface ILogController
  {
    void Exception(String key, String message);

    ILogger GetLogger(String key);

    void Info(String key, String message);
  }

  public class LogController : ILogController
  {
    private IDictionary<String, ILogger> loggers;

    public LogController(Dictionary<String, ILogger> loggers)
    {
      this.loggers = loggers;
    }

    public void Exception(String key, String message)
    {
      throw new NotImplementedException();
    }

    public ILogger GetLogger(String key)
    {
      throw new NotImplementedException();
    }

    public void Info(String key, String message)
    {
      throw new NotImplementedException();
    }
  }
}
