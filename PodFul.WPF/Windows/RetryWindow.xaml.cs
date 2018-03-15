﻿using System;
using System.Collections.Generic;
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
using PodFul.WPF.Processing;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class RetryWindow : Window
  {
    public RetryWindow(IEnumerable<DownloadJob> jobs)
    {
      InitializeComponent();

      if (jobs == null || jobs.Count() == 0)
      {
        throw new ArgumentException("No jobs passed to RetryWindow cstr");
      }
      
    }

    private void CloseButtonClick(Object sender, RoutedEventArgs e)
    {

    }
  }
}
