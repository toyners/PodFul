
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.Object;

  public class LogController : ILogController
  {
    #region Fields
    private IDictionary<String, ILogger> loggers;
    #endregion

    #region Construction
    public LogController(Dictionary<String, ILogger> loggers)
    {
      loggers.VerifyThatObjectIsNotNull("Parameter 'loggers' is null.");

      foreach (var kv in loggers)
      {
        if (kv.Value == null)
        {
          throw new Exception("Parameter 'loggers' contains null value reference.");
        }
      }

      this.loggers = loggers;
    }
    #endregion

    #region Methods
    public ILogger GetLogger(String key)
    {
      key.VerifyThatObjectIsNotNull("Parameter 'key' is null.");

      if (!this.loggers.ContainsKey(key))
      {
        throw new Exception("LogController does not have Logger matching key '" + key + "'.");
      }

      return this.loggers[key];
    }

    public T GetLogger<T>(String key) where T : class
    {
      var value = GetLogger(key);
      if (value is T)
      {
        return (T)value;
      }

      throw new Exception(String.Format("Type of value ({0}) does not match parameter type ({1}).", value.GetType(), typeof(T)));
    }

    public ILogController Message(String key, String message)
    {
      this.GetLogger(key).Message(message);
      return this;
    }
    #endregion
  }
}
