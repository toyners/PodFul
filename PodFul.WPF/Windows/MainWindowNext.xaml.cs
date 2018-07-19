using System;
using System.Windows;
using System.Windows.Controls;
using PodFul.Library;
using PodFul.WPF.Miscellaneous;
using PodFul.WPF.Processing;
using PodFul.WPF.ViewModel;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for MainWindowNext.xaml
  /// </summary>
  public partial class MainWindowNext : Window
  {
    private Settings settings;
    private IFeedCollectionViewModel feedCollectionViewModel;

    public MainWindowNext(Settings settings, IFeedCollectionViewModel feedCollectionViewModel)
    {
      InitializeComponent();

      this.settings = settings;
      this.feedCollectionViewModel = feedCollectionViewModel;
      this.FeedTree.DataContext = this.feedCollectionViewModel;
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      // Use dialog to get the feed directory and url.
      AddFeedToken addFeedToken;
      if (!this.TryGetFeedCreationData(out addFeedToken))
      {
        return;
      }

      // Open dialog to show progress of adding new feed to the feed processor.
      var addFeedProgressWindow = new AddFeedProgressWindow(this.feedCollectionViewModel, addFeedToken);
      addFeedProgressWindow.Owner = this;
      addFeedProgressWindow.ShowDialog();
      Feed feed = addFeedProgressWindow.Feed;
      if (feed == null)
      {
        // Cancelled or Faulted - nothing more to be done.
        return;
      }
    }

    private Boolean TryGetFeedCreationData(out AddFeedToken addFeedToken)
    {
      addFeedToken = default(AddFeedToken);

      var addFeedWindow = new AddFeedWindow();
      addFeedWindow.Owner = this;
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return false;
      }

      addFeedToken = new AddFeedToken(addFeedWindow.FeedDirectory, addFeedWindow.FeedURL, "");
      return true;
    }

    private void FullScanButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void RemoveFeedClick(Object sender, RoutedEventArgs e)
    {
      /*var dialogResult = MessageBox.Show(String.Format("Remove '{0}'?", this.currentFeed.Title), "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
      if (dialogResult == MessageBoxResult.No)
      {
        return;
      }*/
    }

    private void SettingsButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void NextPageClick(Object sender, RoutedEventArgs e)
    {
      var navigation = ((sender as Button).DataContext) as PodcastPageNavigationViewModel;
      navigation.MoveForward();
    }

    private void PreviousPageClick(Object sender, RoutedEventArgs e)
    {
      var navigation = ((sender as Button).DataContext) as PodcastPageNavigationViewModel;
      navigation.MoveBack();
    }

    private void FirstPageClick(Object sender, RoutedEventArgs e)
    {
      var navigation = ((sender as Button).DataContext) as PodcastPageNavigationViewModel;
      navigation.MoveToFirstPage();
    }

    private void LastPageClick(Object sender, RoutedEventArgs e)
    {
      var navigation = ((sender as Button).DataContext) as PodcastPageNavigationViewModel;
      navigation.MoveToLastPage();
    }
  }
}
