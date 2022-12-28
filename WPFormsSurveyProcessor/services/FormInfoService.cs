// Copyright (c) 2022 Mark A. Olbert 
// all rights reserved
// This file is part of WpFormsSurveyProcessor.
//
// WpFormsSurveyProcessor is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// WpFormsSurveyProcessor is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with WpFormsSurveyProcessor. If not, see <https://www.gnu.org/licenses/>.

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
