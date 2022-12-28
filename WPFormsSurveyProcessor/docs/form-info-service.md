# Form Information Service

This service parses a JSON-formatted file of WordPress posts and displays the WPForms forms and their ids contained in the file. All posts other than those defined by WPForms are ignored. All WPForms form definitions, whether or not they are for surveys, are included.

The files are assumed to have been exported by PHPMyAdmin, which creates download files structured in a particular way.

The service works by calling the `ParseFile()` method on an instance of `WpFormsParser`, supplying the path defined on the command line with the **/p** or **/posts** option.

For more information on `WpFormsParser`, consult its [github repository](https://github.com/markolbert/WPFormsSurveyProcessor/blob/master/WpFormsSurvey/docs/readme.md).
