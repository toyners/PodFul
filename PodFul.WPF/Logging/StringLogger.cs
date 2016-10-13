
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class StringLogger : ILogger
  {
    #region Fields
    private readonly FileLogger fileLogger;
    #endregion

    #region Construction
    public StringLogger(FileLogger fileLogger)
    {
      this.fileLogger = fileLogger;
    }
    #endregion

    #region Methods
    public void Exception(String message)
    {
      throw new NotImplementedException();
    }

    public void Message(String message)
    {
      throw new NotImplementedException();
    }

    public void Message(String message, Boolean lineBreak)
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
