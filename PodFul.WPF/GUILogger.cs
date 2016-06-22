
namespace PodFul.WPF
{
  using System;

  public class GUILogger : ILogger
  {
    private readonly ILogger logger;

    public Action<String> PostMessage;

    public GUILogger(ILogger logger)
    {
      this.logger = logger;
    }

    public void Exception(String message)
    {
      this.logger.Exception(message);
    }

    public void Message(String message)
    {
      this.Message(message, true);
    }

    public void Message(String message, Boolean lineBreak)
    {
      this.logger.Message(message, lineBreak);

      if (this.PostMessage != null)
      {
        message += "\r\n";
        this.PostMessage(message);
      }
    }
  }
}
