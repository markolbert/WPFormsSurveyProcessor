using System.Text.Json.Serialization;

namespace J4JSoftware.WpFormsSurvey;

[WpFormsFieldType("file-upload")]
public class FileUploadField : LabeledField
{
    [ JsonPropertyName( "extensions" ) ]
    public string Extensions { get; set; } = string.Empty;
}
