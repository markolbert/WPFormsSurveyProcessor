using J4JSoftware.Configuration.CommandLine;

namespace WPFormsSurveyProcessor;

internal partial class DeusEx
{
    private void SetCommandLineConfiguration( OptionCollection options )
    {
        options.Bind<Configuration, string>(x => x.ConfigurationFilePath, "c", "config")!
            .SetDescription("path to JSON configuration file");

        options.Bind<Configuration, string>( x => x.EntriesFilePath, "e", "entries" )!
               .SetDescription( "path to JSON file containing WpForms entries" );

        options.Bind<Configuration, string>( x => x.PostsFilePath, "p", "posts" )!
               .SetDescription("path to JSON file containing WpForms' WordPress posts");

        options.Bind<Configuration, Exporters>( x => x.ExcelFileInfo.InformationToExport, "s", "scope" )!
               .SetDescription( "information to export" );

        options.Bind<Configuration, string>(x => x.ExcelFileInfo.FileName, "x", "excel")!
               .SetDescription("path to Excel file to be created");

        options.Bind<Configuration, ExcelTimeStamp>(x => x.ExcelFileInfo.TimeStamp, "t", "ts")!
               .SetDescription("define the time stamp, if any, added to the Excel file name");

        options.Bind<Configuration, List<int>>( x => x.FormIds, "f", "formIds" )!
               .SetDescription("one or more WpForms ids to process (empty means process all)");

        options.Bind<Configuration, bool>( x => x.DisplayFormInfo, "i", "formInfo" )!
               .SetDescription("display information about processed forms and/or entries");

        options.Bind<Configuration, bool>( x => x.ShowHelp, "h", "help" )!
               .SetDescription("display help");

        options.Bind<Configuration, bool>(x => x.ShowDocumentation, "d", "docs")!
               .SetDescription("display documentation in browser");
    }
}
