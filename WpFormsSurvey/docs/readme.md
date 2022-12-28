# J4JSoftware.WpFormsSurvey

This library supports parsing the WPForms form descriptions from a WordPress site, and any associated survey responses.

The library repository is available on [github](https://github.com/markolbert/WPFormsSurveyProcessor/blob/master/WpFormsSurvey/docs/readme.md).

The change log is [available here](changes.md).

## Trademarks and Licenses

This program is licensed under the GNU General Public License v3.0. For details on the license, please consult the [license file](../../LICENSE.md).

Excel and various other terms used in the source code and the documentation are copyright [Microsoft Corporation](https://www.microsoft.com).

WPForms and various other terms used in the source code and the documentation are copyright [WPForms, LLC](https://wpforms.com).

## Overview

The library has two sections, one which parses WordPress form definitions and one which parses WPForms' survey responses.

Both sections assume the data is downloaded as JSON from a WordPress site via PHPMyAdmin. Its downloads are structured like this:

```json
[
{
    "type": "header",
    "version": "5.2.0",
    "comment": "Export to JSON plugin for PHPMyAdmin"
},
{
    "type": "database",
    "name": database name
},
{
    "type": "table",
    "name": name of table (wp_posts or wp_wpforms_entries),
    "database": database name,
    "data": 
[
{
    WordPress post definitions or WPForms survey responses
}
```

Because this is an array of heterogenous objects you can't convert it directly to a C# collection using the built-in JSON deserializers. Instead, each JSON object in the array is deserialized twice, first to determine what type of object it is -- based on the value of the `type` field and then to deserialize it to the correct C# entity.

Only WordPress posts with a `post_type` of **wpforms** are parsed. All other post types are skipped.

The next step depends on whether your [parsing WPForms form definitions](parse-forms.md) or [WPForms survey responses](parse-responses.md).

## Converters

The JSON downloaded from PHPMyAdmin uses several formats which aren't C# standard. This necessitates the use of several custom converters, to convert the downloaded JSON values into their C# counterparts.

|Converter|Description|
|---------|-----------|
|`WpDateTimeConverter`|Converts date/time text to a `DateTime` object. Failed conversions result in `DateTime.MinValue`.|
|`WpFormsBooleanConverter`|Converts the text "1" to true, anything else to false|
|`WpFormsIntegerConverter`|Converts textual integer values to numeric. Failed conversions result in 0|
|`WpGuidConverter`|Converts text GUIDs to `Guid`s. Failed converstions result in `Guid.Empty`|
