
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;

  public static class DataGridExtensions
  {
    public static void SelectFirstRows(this DataGrid dataGrid, Int32 count)
    {
      dataGrid.ScrollIntoView(dataGrid.Items[0]);
      dataGrid.SelectedItems.Clear();

      for (Int32 index = 0; index < count; index++)
      {
        var item = dataGrid.Items[index];
        dataGrid.SelectedItems.Add(item);

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

      dataGrid.ScrollIntoView(dataGrid.Items[0]);
    }
  }
}
