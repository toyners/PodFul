
// This namespace must match the namespace of the App.xaml file since this converter is referenced in the xaml
// where full namespaces cannot be used.
namespace PodFul.WPF
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  [ValueConversion(typeof(String), typeof(String))]
  public class TextTruncator : IValueConverter
  {
    public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      var text = value as String;
      if (text == null)
      {
        return String.Empty;
      }

      var limit = Int32.Parse(parameter as String);

      if (text.Length <= limit)
      {
        return text;
      }

      var words = text.Split(' ');

      if (words[0].Length >= limit)
      {
        return text;
      }

      var s = "";
      var index = 0;
      while (s.Length + words[index].Length + 1 <= limit - "...".Length)
      {
        s = s + words[index] + " ";
        index++;
      }

      s += "...";

      return s;
    }

    public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
