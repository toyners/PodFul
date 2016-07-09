
namespace PodFul.WPF
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  [ValueConversion(typeof(DateTime), typeof(String))]
  public class DownloadDateTimeConverter : IValueConverter
  {
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      if (value == null || !(value is DateTime))
      {
        return String.Empty;
      }

      var dateTime = (DateTime)value;

      if (dateTime == DateTime.MinValue)
      {
        return "No download";
      }

      return dateTime.ToString("d-MMM-yyyy H:mm:ss");
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
