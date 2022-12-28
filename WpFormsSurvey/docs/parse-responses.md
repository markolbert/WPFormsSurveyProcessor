# Parsing WPForms Survey Responses

Parsing WPForms survey responses follows a similar pattern to parsing form definitions: walk through each of the responses for a given submission, extract the type of response to determine what kind of C# object to deserialize into, do the deserialization and then initialize the resulting response object.

Determining which specific response class to deserialize into is set by seeing which derived response class is decorated with a matching `WpFormsFieldType` attribute.

As with the form definitions, you can specify additional custom response classes by calling `RegisterEntityType()` or `RegisterEntityTypes()` on the `WpResponsesParser` instance.

Currently there is only one response class which does any kind of initialization, the `LikertResponse` class. It converts the WPForms `value_raw` text property into `LikertScore` objects.

Note that the responses are not linked by reference to their corresponding form fields. That was done so that parsing the form definitions and parsing the responses can be done independantly of each other. However, each response object contains a `FieldId` property which is used by the response's `GetResponseExport()` method to match it to the correct field in a form definition.
