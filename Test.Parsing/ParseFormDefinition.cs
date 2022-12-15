using System.Dynamic;
using System.Text.Json;
using FluentAssertions;
using WpFormsSurvey;
using WPFormsSurvey;

namespace Test.Parsing;

public class ParseFormDefinition : TestBase
{
    [Theory]
    [InlineData("C:\\Users\\mark\\OneDrive - arcabama\\Ardsley73\\surveys\\wp_posts.json")]
    public void Test( string filePath )
    {
        var parser = new WpPostsParser( Logger );
        var result = parser.ParseFile( filePath );

        result.Should().NotBeNull();
        result!.IsValid.Should().BeTrue();
    }
}