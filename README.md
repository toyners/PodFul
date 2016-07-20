# PodFul
**PodFul** is a podcast catcher written in F# and C#.

## 1.0
1. Initial release version using Winforms GUI.

## 2.0
1. Version using WPF GUI.

## 2.1 (1.2)
1. Feed list now contains feed image, title, description and last updated date as an individual entry. Description wrapped and truncated after 100 characters.
2. Download progress now includes percentage (file size is known) or Mb count (file size not known)
3. Special character codes, HTML tags removed or replaced in descriptions.
4. Multi-byte characters in descriptions handled correctly.
5. Logging improved.
6. Various bugfixes.

## 2.1.1 (1.2)
1. Fixed bug where exceptions thrown in feed scanning thread were not being displayed to the user on screen or in the log.
2. Fixed bug where syncing podcasts while adding a feed was not working.
3. Fixed bug when updating a feed that has no podcasts. 

## 2.2 (1.3)
1. Podcast selection window now uses the same list format as the feed list.
2. Feed list has context menu for downloading podcasts, scanning and removing. Double click brings up podcast selection window.
3. New feed properties window available - accessed via context menu.
4. Podcast download date now converted to human readable form.
5. Mouse wheel scrolling in both feed and podcast lists.
6. Last Updated date for feed only changes when the RSS content contains new podcasts.
7. Last Updated date and podcast date now updated once scanning is complete - previously has to restart the app to see changes.
8. Podcasts downloaded during scanning is done in oldest first order (same as manual downloading).
9. File storage names now use a GUID instead of index.
10. Scan button now scans all feeds. Scanning one feed can be done using context menu.
11. Fixed a bug where feeds with on podcasts failed scanning next time there was new podcasts.
12. Minor format fix in processing window when manually downloading podcasts.