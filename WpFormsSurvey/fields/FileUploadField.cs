using System.Text.Json.Serialization;

namespace WPFormsSurvey;

public class FileUploadField : TextField
{
    [ JsonPropertyName( "extensions" ) ]
    public string Extensions { get; set; } = string.Empty;
}
