# J4JSoftware.WpFormsSurveyProcessor

|Version|Description|
|:-----:|-----------|
|2.0.0|added capability to modify existing Excel files; [see details below](#200)|
|1.1.1|fixed bug causing app config file to be ignored|
|1.1.0|fix bugs, add new command line option; [see details below](#110)|
|1.0.0|initial release|

## 2.0.0

After using version 1 for a while I quickly found it tedious to update the data used to generate charts and lists in the Excel file. This was caused by how Excel treats replacing worksheets and named ranges within a workbook.

If you simply replace a sheet in an Excel file with a new sheet, references to the old sheet (and its named ranges) will convert to **#REF** errors, and would need to be fixed manually. To avoid this you have to add new sheets and ranges whenever you update the data. That, in turn, requires modifying sheet and range names to avoid *naming collisions*, which occur when a sheet or range's name matches one already defined in the workbook.

When modifying an existing file, if an exported worksheet will cause a name collision, the newly-exported sheet's name will have a numeric suffix added to it. The suffix will be the first integer value which defines a unique name. For example, if the file contains a sheet named *responses*, the exported responses sheet will be named *responses2* (a suffix of 1 is never assigned). A similar modification is done for named ranges.

By avoiding name collisions you can update existing forumulas by doing a search and replace on the modified file, changing references to the old sheet and range names to the new sheet and range names.

## 1.1.0

Fixed a bug in how the application configuration file was being located at runtime.

Added a new command line option, /l, to enable specifying the logging level used to display messages. Acceptable values are the enumerated values for [Serilog's log event level](https://github.com/serilog/serilog/blob/main/src/Serilog/Events/LogEventLevel.cs). The default is Warning.
