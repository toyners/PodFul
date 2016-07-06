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
6. Various bugfixes