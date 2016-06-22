
namespace PodFul.WPF
{
  using System;

  public interface ILogger
  {
    void Message(String message);

    void Message(String message, Boolean lineBreak);

    void Exception(String message);
  }
}
