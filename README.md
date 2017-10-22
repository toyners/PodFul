# PodFul
**PodFul** is a podcast catcher written in F# and C#.

## 3.1.5 (1.9.6)
1. **Changed:** Improved algorithm for setting the podcast file name to avoid name clashes.
2. **Changed:** Added file size tooltips to manual downloads.
3. **Fixed:** File size tooltip has cleaner format.

## 3.1.4 (1.9.5)
1. **Changed:** Podcast images are resolved when manually downloaded.

## 3.1.3 (1.9.5)
1. **Fixed:** Fixed a bug where scanning feeds but not downloading was not updating the feed file.
2. **Fixed:** Fixed a bug where podcast images are not being set during scanning.

## 3.1.2 (1.9.5)
1. **Fixed:** Fixed a bug where podcast images are not being displayed in podcast list screen.
2. **Changed:** Added blank line after the skipping feed scan message during scanning.

## 3.1.1 (1.9.5)
1. **Fixed:** Fixed a bug when adding feeds with image resolution turned off

## 3.1.0 (1.9.5)
1. **Changed:** Now handling multiple podcasts per item tag.
2. **Changed:** Podcasts can now be posted to delivery points during manual downloads.
3. **Changed:** Last updated value updated when feed is first added.
4. **Fixed:** Fixed bug where right clicking selects rows in the Confirm Podcast Downloads screen.
5. **Fixed:** Fixed issue where the failed podcast count can look misaligned if the cancelled podcast count is 0.
6. **Fixed:** Fixed issue where closing the podcast download or scanning window does not cancel any downloads.
7. **Fixed:** Fixed bug when delivery point has a bad/missing directory.
8. **Fixed:** Scrolling reintroduced in podcast download or scanning window for downloads.
9. **Changed:** Now logging podcast download details.
10. **Changed:** Feed properties (rss, website and directory) can be copied to clipboard.
11. **Changed:** Added high res image for podcast properties window.
12. **Changed:** Error messages now go in their own file.
13. **Changed:** Feed directory can now be changed using directory select dialog.
14. **Changed:** Completed download jobs can now be hidden in the download panels when scanning or manual downloading.
15. **Fixed:** Fixed a bug where CTRL-A in the Confirm Podcast Downloads screen selects wrong rows.
16. **Changed:** Enable downloading of images to be skipped during feed adding. Also changed scanning algorithm so that images are only downloaded when the podcast is downloaded.
17. **Changed:** Feeds can be excluded from full scanning.
18. **Changed:** New podcasts for a feed can be ignored during full scanning (not manual scanning).
19. **Changed:** Podcasts downloaded during full scanning can ignore delivery (not manual scanning).

## 3.0.4 (1.9)
1. **Fixed:** Issue where carriage return was offsetting title when downloading podcast.
2. **Fixed:** Issue where progress value not set to 100% when download is finished.
3. **Fixed:** issue where exception message not displayed correctly in download job display.

## 3.0.3 (1.8)
1. **Fixed:** Adding feeds now places the file in the correct feed directory.

## 3.0.2 (1.8)
1. **Changed:** All main window buttons are now standardised across the application.
2. **Fixed:** All windows open either in the centre of the screen or the centre of their owner.

## 3.0.1 (1.8)
1. **Fixed:** Minor cosmetic tweaks when scanning or downloading.
2. **Fixed:** Bug where COMPLETED was displayed in scanning window title bar when the scanning has been cancelled.

## 3.0 (1.8)
1. **Changed:** Added new Directory Delivery point. Podcasts downloaded during a scan (or manually) can be copied into a single directory for ease of transferring.
2. **Changed:** Settings dialog window completed.
3. **Changed:** Scan dialog window title changes when scanning is cancelled or completed.
4. **Changed:** Podcast Download jobs no longer selectable or highlighted when mousing over.
5. **Changed:** Tidied up visual layout for podcast download jobs.
6. **Changed:** Podcast window opens quicker for feeds with large number of podcasts.

## 2.9 (1.8)
1. **Changed:** Included job counters on scanning window.
2. **Changed:** Automatically scroll job list box when job starts downloading.
3. **Changed:** Can now choose the podcast directory via folder browser dialog when adding feed.
4. **Changed:** Settings now read from and written to disk.
5. **Changed:** Add processing dialog when adding a feed.
6. **Fixed:** Cancelling now works for the scan process and for individual downloads during scanning.

## 2.8 (1.8)
1. **Changed:** During scanning, when a feed has more than a certain number of new podcasts then the user will be presented with a comparison window. 
This window shows the podcasts of the feed both before and after updating.
The user can:
a) Cancel scanning (current feed will not be updated)
b) Skip downloading podcasts (feed will be updated)
c) Choose what podcasts to download (all, new only or an arbitary selection)
2. **Changed:** Minor update to close\cancel button on scanning window.
3. **Changed:** Updated UI so that numbers of waiting, completed, failed download jobs displayed on manual download window.
4. **Fixed:** Now file names based on bad urls are cleaned up e.g. someurl.mp3dest=1672 becomes someurl.mp3
5. **Fixed:** Minor bug in marque progress bar when download is completed.
6. **Fixed:** Minor issue where scan report didn't display all the podcasts that were added to winamp during scanning.
7. **Fixed:** Exception messages being lost when downloading podcasts.

## 2.7 (1.7)
1. **Changed:** Scanning feeds now uses mult-threading to download podcasts while scanning continues.
2. **Changed:** Cancel icon now available for each podcast download.
3. **Fixed:** Cancelling downloads now working as expected.

## 2.6.3 (1.7)
1. **Fixed:** Bug where illegal file name characters are not cleaned up.

## 2.6.2 (1.7)
1. **Fixed:** Bug where updating the feed trashed any local image paths.
2. **Changed:** Can now cancel scanning when images are being downloaded.

## 2.6.1 (1.7)
1. **Fixed:** Podcast download window now has a meaningful title.
2. **Fixed:** Podcasts now delivered to Winamp once downloaded.
3. **Fixed:** Image download message if the download fails.

## 2.6 (1.7)
1. **Changed:** If downloading the image fails then the default image is used.
2. **Fixed:** Minor issues with logging format with regard to image downloading and general exceptions.
3. **Changed:** Podcasts can now be downloaded simultaneously. Number of concurrent downloads can be set in settings window (for application session only).

## 2.5.1 (1.6)
1. **Fixed:** Broken podcast URLs displayed when viewing podcast properties.
2. **Fixed:** Long urls no longer replaced with '...' in the podcast properties window.

## 2.5 (1.6)
1. **Fixed:** Download date was not being updated after manual downloading.
2. **Changed:** Providing feedback when downloading images for feed and podcasts.
3. **Fixed:** Manual synchronisation of podcast files.
4. **Changed:** Tidied up logging so that blank lines are excluded.

## 2.4 (1.5)
1. **Changed:** Added context menu for Podcast window.
2. **Changed:** New podcast properties window available - accessed via context menu.
3. **Changed:** Can synchronise podcast file date/size for a feed - accessed via feed context menu.
4. **Changed:** Added feed image to the feed properties window.
5. **Fixed:** Exceptions were not being handled correctly during manual download.
6. **Changed:** RSS download is now more resilient against transient errors.
7. **Changed:** Minor layout changes to podcast list window.

## 2.3 (1.4)
1. **Fixed:** Processing window title was not being updated correctly.
2. **Changed:** Feed list layout updated to be easier to read - updated date and podcast count right aligned and bold.
3. **Changed:** Enhanced logging around the adding of a feed as part of an ongoing investigation into feed file creation issue.
4. **Fixed:** File size now rounded correctly when converted to readable format.
5. **Changed:** Download progress bar layout improved.
6. **Changed:** Feed file format is now JSON.
7. **Fixed:** Feed Updated string was showing the incorrect tense for yesterday's date.
8. **Changed:** Podcast list now shows podcast thumbnail image.
9. **Changed:** Default image now used for feeds and podcasts that do not have a specified thumbnail image.

## 2.2.1 (1.3)
1. Changed selection mode of podcast selection window to be classic shift/ctrl click for multiple selections.

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
11. Fixed a bug where feeds with no podcasts failed scanning next time there are new podcasts.
12. Minor format fix in processing window when manually downloading podcasts.

## 2.1.1 (1.2)
1. Fixed bug where exceptions thrown in feed scanning thread were not being displayed to the user on screen or in the log.
2. Fixed bug where syncing podcasts while adding a feed was not working.
3. Fixed bug when updating a feed that has no podcasts. 

## 2.1 (1.2)
1. Feed list now contains feed image, title, description and last updated date as an individual entry. Description wrapped and truncated after 100 characters.
2. Download progress now includes percentage (file size is known) or Mb count (file size not known)
3. Special character codes, HTML tags removed or replaced in descriptions.
4. Multi-byte characters in descriptions handled correctly.
5. Logging improved.
6. Various bugfixes.

## 2.0
1. Version using WPF GUI.

## 1.0
1. Initial release version using Winforms GUI.