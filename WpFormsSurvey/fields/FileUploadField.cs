using System.Text.Json.Serialization;

namespace WPFormsSurvey;

[JsonFieldName("file-upload")]
public class FileUploadField : TextField
{
    [ JsonPropertyName( "extensions" ) ]
    public string Extensions { get; set; } = string.Empty;
}
