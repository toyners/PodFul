
// This namespace must match the namespace of the App.xaml file since this converter is referenced in the xaml
// where full namespaces cannot be used.
namespace PodFul.WPF
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  /// <summary>
  /// Converts datetime to user friendly string for use with the feed creation time.
  /// </summary>
  [ValueConversion(typeof(DateTime), typeof(String))]
  public class GeneralDateTimeConverter : IValueConverter
  {
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      if (!(value is DateTime))
      {
        return String.Empty;
      }

      var dateTime = (DateTime)value;

      if (dateTime == DateTime.MinValue)
      {
        return "Never";
      }

      var days = 0;
      var now = DateTime.Now;
      var dt = dateTime;
      while (dt.Year != now.Year || dt.DayOfYear != now.DayOfYear)
      {
        dt = dt.AddDays(1.0);
        days++;
      }

      if (days == 0)
      {
        return String.Format("Today at {0:HH:mm:ss}", dateTime);
      }

      if (days == 1)
      {
        return String.Format("Yesterday at {0:HH:mm:ss}", dateTime);
      }

      if (days < 8)
      {
        return String.Format("{0:dddd} at {0:HH:mm:ss}", dateTime);
      }

      return String.Format("{0:dd-MMM-yyyy HH:mm:ss}", dateTime);
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
