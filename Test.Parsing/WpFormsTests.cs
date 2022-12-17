using System.Dynamic;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
using WpFormsSurvey;

namespace Test.Parsing;

public class WpFormsTests : TestBase
{
    [Theory]
    [InlineData("C:\\Users\\mark\\OneDrive - arcabama\\Ardsley73\\surveys\\wp_posts.json", true)]
    public void ParseFormDefinitions( string filePath, bool hasSurveys )
    {
        var parser = new WpFormsParser( Logger );
        var result = parser.ParseFile( filePath );

        result.Should().NotBeNull();
        result!.Table.Should().NotBeNull();
        result.Table!.Data.Should().NotBeNull();
        result.IsValid.Should().BeTrue();

        var surveyDefs = result.Table!.Data!.Where( x => x.HasSurveyFields ).ToList();
        if( hasSurveys)
            surveyDefs.Should().NotBeEmpty();

        foreach( var choiceField in surveyDefs.SelectMany( x => x.Fields )
                                          .Where( x => x.GetType().IsAssignableTo( typeof( ChoicesField ) ) )
                                          .Cast<ChoicesField>() )
        {
            foreach( var choice in choiceField.Choices )
            {
                choice.Label.Should().NotBeNullOrEmpty();
            }
        }

        foreach (var likertField in surveyDefs.SelectMany(x => x.Fields)
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

    [Theory]
    [InlineData("C:\\Users\\mark\\OneDrive - arcabama\\Ardsley73\\surveys\\wp_posts.json",
                "C:\\Users\\mark\\OneDrive - arcabama\\Ardsley73\\surveys\\wp_wpforms_entries.json")]
    public void ParseResponses(string formsPath, string responsesPath )
    {
        var formsParser = new WpFormsParser(Logger);
        var forms = formsParser.ParseFile(formsPath);

        forms.Should().NotBeNull();
        forms!.Table.Should().NotBeNull();
        forms.Table!.Data.Should().NotBeNull();
        forms.IsValid.Should().BeTrue();

        var sb = new StringBuilder();

        foreach( var formInfo in forms.Table.SummaryInfo )
        {
            sb.Append( $"{formInfo.Id}\t{formInfo.Name}\n" );
        }

        var temp = sb.ToString();
        
        var responsesParser = new WpResponsesParser(Logger);
        var responses = responsesParser.ParseFile( responsesPath );

        responses.Should().NotBeNull();
        responses!.Table.Should().NotBeNull();
        responses.Table!.Data.Should().NotBeNull();
        responses.IsValid.Should().BeTrue();
    }
}