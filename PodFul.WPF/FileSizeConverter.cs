﻿
namespace PodFul.WPF
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Converts byte counts into MB strings. Used for raw values of file sizes.
  /// </summary>
  [ValueConversion(typeof(Int64), typeof(String))]
  public class FileSizeConverter : IValueConverter
  {
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      if (!(value is Int64))
      {
        return String.Empty;
      }

      var fileSize = (Int64)value;

      if (fileSize == 0)
      {
        return "0.0 MB";
      }

      if (fileSize <= 104857)
      {
        return "~0.0 MB";
      }

      var sizeInMb = fileSize / (1048576.0);
      return sizeInMb.ToString("0.0 MB");
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}