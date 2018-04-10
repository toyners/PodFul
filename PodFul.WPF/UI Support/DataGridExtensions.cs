
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using Jabberwocky.Toolkit.WPF;

  public static class DataGridExtensions
  {
    /// <summary>
    /// Select the first n rows of the data grid.
    /// </summary>
    /// <param name="dataGrid">Data grid to select the rows in.</param>
    /// <param name="count">Numbe of rows to select starting from the first row.</param>
    public static void SelectFirstRows(this DataGrid dataGrid, Int32 count)
    {
      dataGrid.SelectedItems.Clear();
      dataGrid.ScrollIntoView(dataGrid.Items[0]);

      for (Int32 rowIndex = 0; rowIndex < count; rowIndex++)
      {
        var item = dataGrid.Items[rowIndex];
        dataGrid.SelectedItems.Add(item);

        DataGridExtensions.SelectDataGridRow(dataGrid, rowIndex);
      }

      dataGrid.ScrollIntoView(dataGrid.Items[0]);
    }

    /// <summary>
    /// Select specific rows of the data grid.
    /// </summary>
    /// <param name="dataGrid">Data grid to select the rows in.</param>
    /// <param name="rowIndexes">Indexes of data grid rows to select.</param>
    public static void SelectSpecificRows(this DataGrid dataGrid, Int32[] rowIndexes)
    {
      dataGrid.SelectedItems.Clear();
      dataGrid.ScrollIntoView(dataGrid.Items[0]);

      for (Int32 index = 0; index < rowIndexes.Length; index++)
      {
        var rowIndex = rowIndexes[index];
        var item = dataGrid.Items[rowIndex];
        dataGrid.SelectedItems.Add(item);

        DataGridExtensions.SelectDataGridRow(dataGrid, rowIndex);
      }

      dataGrid.ScrollIntoView(dataGrid.Items[0]);
    }

    private static void SelectDataGridRow(DataGrid dataGrid, Int32 index)
    {
      DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
      if (row != null)
      {
        DataGridCellsPresenter presenter = row.GetDescendantByType<DataGridCellsPresenter>();
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
