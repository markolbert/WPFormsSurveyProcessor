# Parsing and Exporting Service

Exporting to Excel is handled by a C# [NPOI library](https://github.com/nissl-lab/npoi). That library creates the Excel workbook and worksheets in memory and then allows you to write the result to an .xlsx file.

- [Parsing the Files](#parsing-the-files)
- [Exporting to Excel](#exporting-to-excel)

## Parsing the Files

This service parses two files:

- a JSON-formatted file of WordPress posts, whose path was specified on the command line with the **/p** or **/posts** option; and,
- a JSON-formatted file of WPForms responses, whose path was specified on the command line with the **/e** or **/entries** option.

All posts other than those defined by WPForms are ignored. All WPForms form definitions, whether or not they are for surveys, are included.

The files are assumed to have been exported by PHPMyAdmin, which creates download files structured in a particular way.

The service first parses the WordPress posts file to determine the WPForms form definitions. It does that by calling the `ParseFile()` method on an instance of `WpFormsParser`, supplying the path defined on the command line with the **/p** or **/posts** option.

Next, the service parses the WPForms entries file to determine the user survey responses. It does that by calling the `ParseFile()` method on an instance of `WpResponsesParser`, supplying the path defined on the command line with the **/e** or **/entries** option.

For more information on `WpFormsParser` and `WpResponsesParser`, consult their [github repository](https://github.com/markolbert/WPFormsSurveyProcessor/blob/master/WpFormsSurvey/docs/readme.md).

## Exporting to Excel

Once the form definitions and survey responses have been parsed, one or more exporters are called to create specific spreadsheets in an Excel .xlsx file. The exporters all derive from `ExportBase`, a generic base class defining a number of methods to create and format tabular data and named ranges in an Excel spreadsheet.

Each derived exporter class overrides the `GetRecords()` abstract method to create an instance of the data needed for each row of the data table being created. Rows are created based on those instances in the overriden `ProcessRecord()` method.

The entire export/table creation process is wrapped by two other methods, `StartExport()` and `FinishExport()`.

The overridden `StartExport()` methods typically create the header row for each table, although they can do other initialization work as well.

The overridden `FinishExport()` methods typically autosize one or more table columns and create whatever named ranges were defined for their spreadsheet in the `appConfig.json` file.
