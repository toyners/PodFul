
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;

  public static class RowSelector
  {
    public static void Select(DataGrid dataGrid, Int32 index)
    {
      DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
      if (row != null)
      {
        DataGridCellsPresenter presenter = VisualTreeLocator.FindDataGridCellsPresenter(row);
        if (presenter != null)
        {
          DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
          if (cell != null)
          {
            cell.Focus();
          }
        }
      }
    }
  }
}
