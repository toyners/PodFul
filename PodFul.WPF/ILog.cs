﻿
namespace PodFul.WPF
{
  using System;

  public interface ILog
  {
    void Message(String message);

    void Exception(String message);
  }
}
