
namespace PodFul.WPF.Logging
{
  using System;
  using System.Text;

  public class MessageBuilder : ILogger
  {
    private StringBuilder builder = new StringBuilder(500);

    public String Text
    {
      get
      {
        return this.builder.ToString();
      }
    }

    public void Message(String message)
    {
      this.builder.Append(message);
    }
  }
}
