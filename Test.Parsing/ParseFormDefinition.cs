using System.Text.Json;
using WpFormsSurvey;

namespace Test.Parsing;

public class ParseFormDefinition : TestBase
{
    [Theory]
    [InlineData("C:\\Users\\mark\\OneDrive - arcabama\\Ardsley73\\surveys\\wp_wpforms_entries.json")]
    public void Test( string filePath )
    {
        var options = new JsonSerializerOptions { Converters = { new FormDefinitionConverterFactory(Logger) } };

        var parsed = JsonSerializer.Deserialize<(string.Empty, options);
    }
}