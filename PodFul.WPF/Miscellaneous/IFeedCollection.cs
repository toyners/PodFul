﻿
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using Library;

  public interface IFeedCollection
  {
    Feed this[Int32 index] { get; set; }
    Int32 Count { get; }

    void AddFeed(Feed feed);
    void RemoveFeed(Feed feed);
    void RemoveFeed(Int32 index);
    void UpdateFeedContent(Feed feed);
  }
}
