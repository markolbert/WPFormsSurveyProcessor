# J4JSoftware.WPFormsSurvey

This repository contains two projects, a library for parsing and exporting WPForms form definitions and survey responses, and an executable which uses that library to export information to an Excel (.xlsx) file.

|Project|Description|
|-------|-----------|
|[WpFormsSurvey](WpFormsSurvey/docs/readme.md)|The parsing library|
|[WpFormsSurveyProcessor](WpFormsSurveyProcessor/docs/readme.md)|The console program|

## License, Copyrights, etc

The library and program are licensed under the GNU General Public License v3.0. For details on the license, please consult the [license file](LICENSE.md).

Excel and various other terms used in the source code and the documentation are copyright [Microsoft Corporation](https://www.microsoft.com).

WPForms and various other terms used in the source code and the documentation are copyright [WPForms, LLC](https://wpforms.com).

## TL; DR

Assuming the folder containing the exeutable is accessible via your PATH:

```cmd
WPFormsSurveyProcessor /p path-to-wp-posts.json-file 
/e path-to-wpforms-entries.json-file
```

If the export is successful, this will create a time-stamped file `Survey Results YYYY-MM-DD HH-MM-SS.xlsx` in the current directory.

For further details on using the program please consult the [detailed usage instructions](WPFormsSurveyProcessor/docs/readme.md).
