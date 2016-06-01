﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for SelectionWindow.xaml
  /// </summary>
  public partial class SelectionWindow : Window
  {
    private enum SelectRowsType
    {
      None,
      All,
    }

    public SelectionWindow(String title)
    {
      InitializeComponent();

      this.Title = title;
    }

    private void ClearButton_Click(Object sender, RoutedEventArgs e)
    {
      this.SelectRows(SelectRowsType.None);
    }

    private void AllButton_Click(Object sender, RoutedEventArgs e)
    {
      this.SelectRows(SelectRowsType.All);
    }

    private void Window_Closing(Object sender, CancelEventArgs e)
    {
      this.DialogResult = false;
    }

    private void SelectRows(SelectRowsType selectRowsType)
    {
      if (selectRowsType == SelectRowsType.All)
      {
        this.ItemGrid.SelectAll();
        return;
      }

      this.ItemGrid.UnselectAll();
    }
    
    private void ItemGrid_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      this.startButton.IsEnabled = e.AddedItems.Count > 0;
    }
  }
}
