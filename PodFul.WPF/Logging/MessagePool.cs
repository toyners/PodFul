
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  public class MessagePool : ILogger, IEnumerator, IEnumerable
  {
    #region Fields
    private List<String> messages = new List<String>();

    private Int32 enumeratorIndex = -1;
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
    public void Message(String message)
    {
      this.messages.Add(message);
    }
    
    public Boolean MoveNext()
    {
      this.enumeratorIndex++;
      return (this.enumeratorIndex < this.messages.Count);
    }

    public void Reset()
    {
      this.enumeratorIndex = -1;
    }

    public IEnumerator GetEnumerator()
    {
      return this;
    }
    #endregion
  }
}
