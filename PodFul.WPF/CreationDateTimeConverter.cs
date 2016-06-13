
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
  public class CreationDateTimeConverter : IValueConverter
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
        return String.Empty;
      }

      var timeSpan = DateTime.Now - creationDateTime;

      if (timeSpan.Days == 0)
      {
        return String.Format("Last Updated: Today at {0:HH:mm:ss}", creationDateTime);
      }

      if (timeSpan.Days == 1)
      {
        return String.Format("Last Updated: Yesterday at {0:HH:mm:ss}", creationDateTime);
      }

      if (timeSpan.Days < 8)
      {
        return String.Format("Last Updated: {0:dddd} at {0:HH:mm:ss}", creationDateTime);
      }

      return String.Format("Last Updated: {0:dd-MMM-yyyy HH:mm:ss}", creationDateTime);
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
