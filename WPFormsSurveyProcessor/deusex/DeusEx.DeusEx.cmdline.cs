using J4JSoftware.Configuration.CommandLine;

namespace WPFormsSurveyProcessor;

internal partial class DeusEx
{
    private void CommandLineConfiguration( OptionCollection options )
    {
        options.Bind<Configuration, string>( x => x.EntriesFilePath, "e", "entries" )!
               .SetDescription( "path to JSON file containing WpForms entries" );

        options.Bind<Configuration, string>( x => x.PostsFilePath, "p", "posts" )!
               .SetDescription("path to JSON file containing WpForms' WordPress posts");

        options.Bind<Configuration, string>(x => x.ExcelFilePath, "x", "excel")!
               .SetDescription("path to Excel file to be created");

        options.Bind<Configuration, List<int>>( x => x.FormIds, "f", "formIds" )!
               .SetDescription("one or more WpForms ids to process (empty means process all)");

        options.Bind<Configuration, bool>( x => x.DisplayFormInfo, "d", "displayFormInfo" )!
               .SetDescription("display information about processed forms and/or entries");

        options.Bind<Configuration, bool>( x => x.ShowHelp, "h", "help" )!
               .SetDescription("display help");
    }
}
