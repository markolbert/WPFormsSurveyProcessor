# J4JSoftware.WpFormsSurveyProcessor

*These are the usage instructions for the executable. Developer information for the code base is [available here](developer.md).*

*The change log is [available here](changes.md)*.

- [Overview](#overview)
- [Command Line Options](#command-line-options)
- [Export File Name](#export-file-name)
- [Modification of Existing Excel File](#modification-of-existing-excel-file)
- [Defining Named Ranges](#defining-named-ranges)
- [Creating the Required Files](#creating-the-required-files)
  - [Creating the Forms Definition File](#creating-the-forms-definition-file)
  - [Creating the User Entries File](#creating-the-user-entries-file)

## Overview

**WpFormsSurveyProcessor** exports WordPress WPForms form and survey response data to an Excel (xlsx) file. You can export one or more of the following data sets, each of which will be on a separate worksheet (the bolded terms refer to the command line values used to restrict output to certain datasets):

- WpForms id and name (**FormInformation**)
- Field descriptions, including form and field ids (**FieldDescriptions**)
- Details on choices for choice fields (e.g., checkbox, radio) (**FieldDescriptions**)
- User responses/submissions (**Submissions**)

You can also export all the datasets using the command line term **All** (which is the default and need not be specified).

## Command Line Options

The option flags are case insensitive.

|Option|Description|
|:----:|-----------|
|/h, /help|Display simple help information|
|/d, /docs|Display this documentation in a browser|
|/l, /logLevel|Adjust the minimum level of logging events displayed to the console. The default is Warning.|
|/c, /config|Path to the application configuration file. If not specified, the default *appConfig.json* file in the application directory is used|
|/p, /posts|Path to the file containing the WordPress posts. This is where WpForms stores form definitions. See below for details on creating this file.|
|/e, /entries|Path to the file containing the WpForms user entries. See below for details on creating this file.|
|/x, /excel|Path to the export file created by the program. *Warning: existing files will be overwritten without confirmation.*|
|/s, /scope|Defines the information/worksheets to be exported. See below for details.|
|/f, /formIds/|A list of form ids to export. If not specified, all forms will be exported.|
|/i, /formInfo|Displays a list of form names and ids contained in the provided WordPress posts file.|

The /f and /s options allow multiple values to be provided, separated by either commas or spaces.

## Export File Name

When you specify an export file name it is checked to ensure it can be written. If for some reason the path you supply is not, the export file name will be changed to something that is writable. This could result in the export file being written to the current directory, so if you don't find the file where you expect it, review the actual name of the file being written. That's echoed to the console when it is created.

## Modification of Existing Excel File

If the Excel file you specify does not exist a new file is created. However, if the file *does* exist it is *modified* rather than being replaced.

When modifying an existing file, if an exported worksheet will cause a *name collision* (i.e., the sheet's name matches an existing sheet in the file), the newly-exported sheet's name will have a numeric suffix added to it. The suffix will be the first integer value which defines a unique name. For example, if the file contains a sheet named *responses*, the exported responses sheet will be named *responses2* (a suffix of 1 is never assigned).

Similarly, the name of a named range will be modified in the same fashion to avoid naming collisions.

This approach is used to simplify updating an existing Excel file with new data. Unfortunately, if you simply replace a sheet in an Excel file with a new sheet, references to the old sheet (and its named ranges) will convert to **#REF** errors, and would need to be fixed manually. By avoiding name collisions you can update existing forumulas by doing a search and replace on the modified file, changing references to the old sheet and range names to the new sheet and range names.

## Defining Named Ranges

The default *appConfig.json* configuration file defines a number of named ranges that are created when data is exported. You can define your own by modifying the file, which is located in the directory/folder where you installed the application.

Here are the default settings:

```json
{
  "WorksheetDefinitions": [
    {
      "SheetType": "Forms",
      "SheetName": "forms",
      "Ranges": [
        {
          "Name": "FormIds",
          "Context": "Worksheet",
          "FirstColumn": "A"
        },
        {
          "Name": "FormNames",
          "Context": "Worksheet",
          "FirstColumn": "A",
          "LastColumn": "B"
        }
      ]
    },
    {
      "SheetType": "Fields",
      "SheetName": "fields",
      "Ranges": [
        {
          "Name": "Fields",
          "Context": "Worksheet",
          "FirstColumn": "C",
          "LastColumn": "E"
        }
      ]
    },
    {
      "SheetType": "Choices",
      "SheetName": "choices",
      "Ranges": [
        {
          "Name": "Choices",
          "Context": "Worksheet",
          "FirstColumn": "E",
          "LastColumn": "F"
        }
      ]
    },
    {
      "SheetType": "Responses",
      "SheetName": "responses",
      "Ranges": [
        {
          "Name": "FieldKeys",
          "Context": "Worksheet",
          "FirstColumn": "H"
        },
        {
          "Name": "SubfieldKeys",
          "Context": "Worksheet",
          "FirstColumn": "I"
        },
        {
          "Name": "ResponseIndices",
          "Context": "Worksheet",
          "FirstColumn": "J"
        },
        {
          "Name": "Responses",
          "Context": "Worksheet",
          "FirstColumn": "K"
        }
      ]
    }
  ]
}
```

Each exportable spreadsheet is configured through an instance of `WorksheetInfo`. The collection of `WorksheetInfo` objects is contained in a property of the `Configuration` object governing how the program runs:

```csharp
public List<WorksheetInfo> WorksheetDefinitions { get; set; } = new();
```

A `WorksheetInfo` object contains information about the type of worksheet it defines, the base name to be given to the sheet when it is created and any named ranges it may define:

```csharp
 public class WorksheetInfo
{
  public SheetType SheetType { get; set; } = SheetType.Undefined;
  public string SheetName { get; set; } = string.Empty;
  public List<NamedRangeConfiguration> Ranges { get; set; } = new();

  public bool HasRanges => Ranges.Any();

  public bool IsValid();
}
```

The full syntax for the named range object looks like this:

```json
{
    "Name": range's name,
    "Context": the value Worksheet or Workbook (see below),
    "FirstColumn": the letter of the column starting the range,
    "LastColumn": the letter ending the range (see below),
    "IncludeHeader": true or false (see below)
}
```

For **Context**, Worksheet means the range is defined at the worksheet level and you'll need to refer to it as *sheetname!range-name*. Workbook means the range is defined at the workbook level, and doesn't require a prefix to be referenced.

**LastColumn** is optional. If omitted, the named range will be one column wide (i.e., the last column will be the same as the first).

**IncludeHeader** controls whether or not the header row is included in the named range. By default it's false and the header is *not* included.

## Creating the Required Files

**WpFormsSurveyProcessor** requires at least a definition of the WpForms objects to display information about the forms. To export data, however, it also requires the user submissions. Both of these files can be exported from the database used by you WordPress website.

There are various ways to do this, depending on how comfortable you are with directly accessing that database. However, the simpler approach is to use **PhpMyAdmin**, if it's available for your site. These directions use that approach.

### Creating the Forms Definition File

Navigate to the *wp_posts* table, which WordPress uses to contain all of the posts in your site, which includes the definitions of your WPForms forms.

![navigate to wp_posts](assets/nav2wpposts.png)

The wp_posts table contains entries like the following:

![wp_posts entries](assets/posts.png)

You could simply export this entire file and process it. But it's likely to be quite large, so a better way is to extract only the posts created by WPForms. You can do that by clicking on the **SQL** tab at the top of the right-hand pane and modifying the default filter condition. After making the change, click the Go button:

![filtering posts](assets/filter-posts.png)

Now click the **Export** tab at the top of the right-hand pane, select JSON as the output format, and click the Go button.

![export JSON](assets/export-json.png)

You can also format the JSON in human-friendly form if you want to, although that's not required.

The export generates a file you download and save someplace where **WpFormsSurveyProcessor** can access it.

### Creating the User Entries File

Navigate to the *wp_wpforms_entries* table, which WPForms uses to contain survey responses. It will look something like this:

![user entries](assets/survey-entries.png)

As with the wp_forms table, you can export this entire table and process it. But if you only want the information for one or a few surveys it's worth filtering the table before exporting it. You do this by changing the WHERE condition on the SQL tab in the right-hand pane:

![filtering user entries](assets/filter-responses.png)

The rest of the export process mimics what was described for creating the forms definition file: select JSON as the export format, click Export, and save the downloaded file someplace where **WpFormsSurveyProcess** can access it.
