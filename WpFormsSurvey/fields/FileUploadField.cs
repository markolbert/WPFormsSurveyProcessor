using System.Text.Json.Serialization;

namespace WpFormsSurvey;

[WpFormsFieldType("file-upload")]
public class FileUploadField : TextField
{
    [ JsonPropertyName( "extensions" ) ]
    public string Extensions { get; set; } = string.Empty;
}
