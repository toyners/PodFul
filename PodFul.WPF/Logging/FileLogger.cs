
namespace PodFul.WPF.Logging
{
  using System;
  using NLog;

  public class FileLogger : ILogger
  {
    protected static Logger logger = LogManager.GetCurrentClassLogger();
    private const String lineBreakText = "\r\n";

    public virtual void Message(String message)
    {
      logger.Info(message);
    }
  }
}
