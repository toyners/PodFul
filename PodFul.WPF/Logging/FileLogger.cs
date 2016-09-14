
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
      FileLogger.VerifyMessageIsNotBlank(message, true);
      logger.Error(message);
    }

    public void Message(String message)
    {
      // No linebreak by default.
      this.Message(message, false);
    }

    public void Message(String message, Boolean lineBreak)
    {
      FileLogger.VerifyMessageIsNotBlank(message, false);
      logger.Info(message + (lineBreak ? lineBreakText : null));
    }

    private static void VerifyMessageIsNotBlank(String message, Boolean isException)
    {
      if (String.IsNullOrEmpty(message))
      {
        var err = "Message is empty or null for " + (isException ? " Exception" : "Information");
        throw new Exception(err);
      }
    }
  }
}
