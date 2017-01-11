
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Runtime.CompilerServices;

  public class NotifyPropertyChangedBase
  {
    /// <summary>
    /// Set the field to the new value if it is different and then raises the property changed event handler.
    /// </summary>
    /// <typeparam name="T">Type of the field and value</typeparam>
    /// <param name="fieldValue">The existing field value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="propertyName">Name of the property being changed. Uses the name of the calling method by default.</param>
    protected void SetField<T>(ref T fieldValue, T newValue, PropertyChangedEventHandler propertyChangedEventHandler, [CallerMemberName] String propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(fieldValue, newValue))
      {
        return;
      }

      fieldValue = newValue;
      propertyChangedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
