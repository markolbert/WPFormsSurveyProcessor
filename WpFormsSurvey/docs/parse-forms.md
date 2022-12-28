# Parsing WordPress Posts

The first step in parsing a WPForms form definition is to convert the `post_content` field into a `JSONElement` so we can navigate through it and extract the field definitions.

That navigation is complicated by the fact that some WPForms form definitions structure the field information as a JSON array while others structure it as a series of JSON properties.

Either way, we step through the entities and do a simple JSON deserialization which extracts the field's type identifier and returns the field definition's value as text. That type information defines which type of C# object we deserialize the field definition text into.

## Parsing Field Definitions

Determining what type of C# object to deserialize a field definition into is based on the `WpFormsFieldType` attributes which decorate C# field classes. When it initializes, the library includes all the field classes decorated with such attributes which have public parameterless constructors (a requirement of the JSON deserialization process). You can add additional customized field classes by calling `RegisterEntityType()` or `RegisterEntityTypes()` on the `WpFormsParser` instance.

Assuming a WPForms field is successfully deserialized, it's configuration is finalized by a call to `FieldBase::Initialize()`. The initialization always stores a reference to the `Form` object the field belongs to, but it may involve other things as well depending on the specific field type.

For example, `select`, `radio` and `checkbox` field definitions contain multiple choices for the survey respondent to select. These are deserialized into the property `RawChoices`, which is a `JSONElement`. The overridden `Initialize()` method then processes `RawChoices` into a collection of `FieldChoice` objects.
