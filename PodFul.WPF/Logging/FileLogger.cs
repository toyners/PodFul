
namespace PodFul.WPF
{
  using System;
  using NLog;

  public class FileLogger : ILogger
  {
    private static Logger logger = LogManager.GetCurrentClassLogger();
    private const String lineBreakText = "\r\n";

    public void Exception(String message)
    {
      logger.Error(message);
    }

    public void Message(String message)
    {
      // No linebreak by default.
      this.Message(message, false);
    }

    public void Message(String message, Boolean lineBreak)
    {
      logger.Info(message + (lineBreak ? lineBreakText : null));
    }
  }
}
