
namespace PodFul.WPF.Logging
{
  using System;

  public class ExceptionLogger : FileLogger
  {
    public override void Message(String message)
    {
      FileLogger.logger.Error(message);
    }
  }
}
