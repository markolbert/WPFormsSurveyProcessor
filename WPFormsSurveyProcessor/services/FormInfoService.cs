using Alba.CsConsoleFormat;
using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace J4JSoftware.WpFormsSurvey;

internal class FormInfoService : ServiceBase
{
    public FormInfoService(
        Configuration config,
        IJ4JLogger logger
    )
        : base( config, logger )
    {
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        if( Configuration.PostsFileStatus == Configuration.FileStatus.Okay )
            ParseFormDefinitions();
        else ParseFormResponses();

        return Task.CompletedTask;
    }

    private void ParseFormDefinitions()
    {
        var parser = J4JDeusEx.ServiceProvider.GetRequiredService<WpFormsParser>();
        var forms = parser.ParseFile( Configuration.PostsFilePath );

        if( forms?.Table == null )
        {
            Logger.Error( "Failed to parse WordPress posts file" );
            return;
        }

        var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

        var doc = new Document( new Span( "WpForms Form Info" ) { Color = ConsoleColor.Magenta },
                                new Grid
                                {
                                    Color = ConsoleColor.Gray,
                                    Columns = { GridLength.Auto, GridLength.Auto },
                                    Children =
                                    {
                                        new Cell( "Id" ) { Stroke = headerThickness },
                                        new Cell( "Title" ) { Stroke = headerThickness },
                                        forms!.Table!.ToFormInfo().Select( x => new[]
                                        {
                                            new Cell( x.Id ), new Cell( x.Name )
                                        } )
                                    }
                                } );

        ConsoleRenderer.RenderDocument( doc );
    }

    private void ParseFormResponses()
    {
        var parser = J4JDeusEx.ServiceProvider.GetRequiredService<WpResponsesParser>();
        var responses = parser.ParseFile(Configuration.EntriesFilePath);

        if (responses?.Table == null)
            Logger.Error("Failed to parse WordPress posts file");

        var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

        var doc = new Document( new Span( "WpForms Form Info" ) { Color = ConsoleColor.Magenta },
                                new Grid
                                {
                                    Color = ConsoleColor.Gray,
                                    Columns = { GridLength.Auto, GridLength.Auto },
                                    Children =
                                    {
                                        new Cell( "Id" ) { Stroke = headerThickness },
                                        new Cell( "Title" ) { Stroke = headerThickness },
                                        responses!.Table!.FormIds.Select( x => new[]
                                        {
                                            new Cell( x )
                                        } )
                                    }
                                } );

        ConsoleRenderer.RenderDocument( doc );
    }
}
