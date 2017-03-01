﻿
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.Object;

  public class LogController : ILogController
  {
    private IDictionary<String, ILogger> loggers;

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

    public ILogger GetLogger(String key)
    {
      key.VerifyThatObjectIsNotNull("Parameter 'key' is null.");

      if (!this.loggers.ContainsKey(key))
      {
        throw new Exception("LogController does not have Logger matching key '" + key + "'.");
      }

      return this.loggers[key];
    }

    public T GetLogger<T>(String key)
    {
      throw new NotImplementedException();
    }

    public void Message(String key, String message)
    {
      this.GetLogger(key).Message(message);
    }
  }
}
