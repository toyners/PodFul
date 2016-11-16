
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Windows;
  using System.Windows.Controls.Primitives;
  using System.Windows.Media;

  public static class VisualTreeLocator
  {
    public static DataGridCellsPresenter FindDataGridCellsPresenter(DependencyObject obj)
    {
      var count = VisualTreeHelper.GetChildrenCount(obj);
      for (Int32 i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);

        if (child is DataGridCellsPresenter)
        {
          return (DataGridCellsPresenter)child;
        }

        child = FindDataGridCellsPresenter(child);

        if (child != null)
        {
          return (DataGridCellsPresenter)child;
        }
      }

      return null;
    }
  }
}
