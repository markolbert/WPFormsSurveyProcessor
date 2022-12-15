using System.Dynamic;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
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
        result!.Table.Should().NotBeNull();
        result.Table!.Data.Should().NotBeNull();
        result.IsValid.Should().BeTrue();

        foreach( var choiceField in result.Table.Data!.SelectMany( x => x.Fields )
                                          .Where( x => x.GetType().IsAssignableTo( typeof( ChoicesField ) ) )
                                          .Cast<ChoicesField>() )
        {
            foreach( var choice in choiceField.Choices )
            {
                choice.Label.Should().NotBeNullOrEmpty();
            }
        }

        foreach (var likertField in result.Table.Data!.SelectMany(x => x.Fields)
                                          .Where(x => x.GetType().IsAssignableTo(typeof(LikertField)))
                                          .Cast<LikertField>())
        {
            if( likertField.SingleRow )
                likertField.Rows.Should().BeEmpty();
            else
            {
                likertField.Rows.Should().NotBeEmpty();

                foreach( var row in likertField.Rows )
                {
                    row.Should().NotBeNullOrEmpty();
                }
            }

            likertField.Columns.Should().NotBeEmpty();
            
            foreach (var column in likertField.Columns)
            {
                column.Should().NotBeNullOrEmpty();
            }
        }
    }
}