
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

      var creationDateTime = (DateTime)value;

      if (creationDateTime == DateTime.MinValue)
      {
        return "Never";
      }

      var days = 0;
      var now = DateTime.Now;
      var dateTime = creationDateTime;
      while (dateTime.Year != now.Year || dateTime.DayOfYear != now.DayOfYear)
      {
        dateTime = dateTime.AddDays(1.0);
        days++;
      }
      //var timeSpan = DateTime.Now - creationDateTime.;

      if (days == 0)
      {
        return String.Format("Today at {0:HH:mm:ss}", creationDateTime);
      }

      if (days == 1)
      {
        return String.Format("Yesterday at {0:HH:mm:ss}", creationDateTime);
      }

      if (days < 8)
      {
        return String.Format("{0:dddd} at {0:HH:mm:ss}", creationDateTime);
      }

      return String.Format("{0:dd-MMM-yyyy HH:mm:ss}", creationDateTime);
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
