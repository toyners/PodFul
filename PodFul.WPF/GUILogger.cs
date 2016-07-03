
namespace PodFul.WPF
{
  using System;

  public class GUILogger : ILogger
  {
    private const String lineBreakText = "\r\n";
    private readonly FileLogger fileLogger;

    public Action<String> PostMessage;

    public GUILogger(FileLogger logger)
    {
      this.fileLogger = logger;
    }

    public void Exception(String message)
    {
      this.fileLogger.Exception(message);
      this.Post(message, false);
    }

    public void Message(String message)
    {
      this.Message(message, true);
    }

    public void Message(String message, Boolean lineBreak)
    {
      this.fileLogger.Message(message);
      this.Post(message, lineBreak);
    }

    private void Post(String message, Boolean lineBreak)
    {
      if (this.PostMessage != null)
      {
        message += (lineBreak ? lineBreakText : null);
        this.PostMessage(message);
      }
    }
  }
}
