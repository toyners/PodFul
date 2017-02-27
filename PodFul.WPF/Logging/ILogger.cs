
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections.Generic;

  public interface ILogger
  {
    void Message(String message);

    void Message(String message, Boolean lineBreak);

    void Exception(String message);
  }
}
