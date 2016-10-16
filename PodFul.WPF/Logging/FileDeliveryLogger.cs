
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  public class FileDeliveryLogger : ILogger, IEnumerator, IEnumerable
  {
    #region Fields
    private List<String> messages = new List<String>();

    private Int32 enumeratorIndex;
    #endregion

    #region Properties
    public Object Current
    {
      get
      {
        return this.messages[this.enumeratorIndex];
      }
    }
    #endregion

    #region Methods
    public void Exception(String message)
    {
      this.Message(message);
    }

    public void Message(String message)
    {
      this.messages.Add(message);
    }

    public void Message(String message, Boolean lineBreak)
    {
      this.Message(message);
    }

    public Boolean MoveNext()
    {
      this.enumeratorIndex++;
      return (this.enumeratorIndex < this.messages.Count);
    }

    public void Reset()
    {
      this.enumeratorIndex = 0;
    }

    public IEnumerator GetEnumerator()
    {
      return this;
    }
    #endregion
  }
}
