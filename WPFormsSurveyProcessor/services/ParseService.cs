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

using J4JSoftware.DependencyInjection;
using J4JSoftware.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Math.EC;

namespace J4JSoftware.WpFormsSurvey;

internal class ParseService : ServiceBase
{
    private readonly WpFormsParser _formsParser;
    private readonly WpResponsesParser _responsesParser;
    private readonly ExportFormInfo _formInfoExporter;
    private readonly ExportChoiceFields _choiceFieldsExporter;
    private readonly ExportFieldDescriptions _fieldDescriptionsExporter;
    private readonly ExportResponses _responsesExporter;

    public ParseService(
        Configuration config,
        WpFormsParser formsParser,
        WpResponsesParser responsesParser,
        ExportFormInfo formInfoExporter,
        ExportChoiceFields choiceFieldsExporter,
        ExportFieldDescriptions fieldDescriptionsExporter,
        ExportResponses responsesExporter,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
        _formsParser = formsParser;
        _responsesParser = responsesParser;

        _formInfoExporter = formInfoExporter;
        _choiceFieldsExporter = choiceFieldsExporter;
        _fieldDescriptionsExporter = fieldDescriptionsExporter;
        _responsesExporter = responsesExporter;
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        if( Configuration.Workbook == null )
        {
            Logger.Error("Workbook is not configured");
            return Task.CompletedTask;
        }

        var formsDownload = ParseForms();
        if( formsDownload?.Data is not {} formDefinitions )
            return Task.CompletedTask;

        Logger.Information("Parsed forms definition(s)");

        if( Configuration.FormIds.Any() )
        {
            formDefinitions = formsDownload.Data
                                           .Where( x => Configuration.FormIds.Any( y => x.Id == y ) )
                                           .ToList();

            if( !formDefinitions.Any() )
            {
                Logger.Warning( "No form(s) with that/those form ids were found" );
                return Task.CompletedTask;
            }
        }

        var responsesDownload = ParseResponses();
        if( responsesDownload?.Data is not {} individualResponses )
            return Task.CompletedTask;

        Logger.Information("Parsed user responses");

        if ( Configuration.FormIds.Any() )
        {
            individualResponses = individualResponses
                                   .Where( x => Configuration.FormIds.Any( y => x.FormId == y ) )
                                   .ToList();

            if( !individualResponses.Any() )
            {
                Logger.Warning( "No submissions(s) with that/those form ids were found" );
                return Task.CompletedTask;
            }
        }

        if( ( Configuration.InformationToExport & Exporters.FormInformation ) == Exporters.FormInformation
        && !ExportFormInfo( formDefinitions ) )
            return Task.CompletedTask;

        if( ( Configuration.InformationToExport & Exporters.FieldDescriptions ) == Exporters.FieldDescriptions
        && !ExportFieldDescriptions( formDefinitions ) )
            return Task.CompletedTask;

        if( ( Configuration.InformationToExport & Exporters.ChoiceFields ) == Exporters.ChoiceFields
        && !ExportChoiceFields( formDefinitions ) )
            return Task.CompletedTask;

        if( ( Configuration.InformationToExport & Exporters.Responses ) == Exporters.Responses
        && !ExportResponses( new ResponseInfo( formDefinitions, individualResponses ) ) )
            return Task.CompletedTask;

        Logger.Information<string>("Writing results to {0}", Configuration.ExcelPath);

        try
        {
            using var fs = File.OpenWrite( Configuration.ExcelPath );
            Configuration.Workbook!.Write( fs, false );

            Logger.Information("Done");
        }
        catch( IOException ioException )
        {
            var temp = FileLocking.WhoIsLocking( Configuration.ExcelPath );
            Logger.Error( ioException.Message );
        }

        return Task.CompletedTask;
    }

    private Forms? ParseForms()
    {
        var retVal = _formsParser.ParseFile(Configuration.PostsFilePath);
        if( retVal?.Table != null )
            return retVal.Table;

        Logger.Error("Failed to parse WordPress posts file");
        return null;
    }

    private Entries? ParseResponses()
    {
        var retVal = _responsesParser.ParseFile(Configuration.EntriesFilePath);
        if (retVal?.Table != null)
            return retVal.Table;

        Logger.Error("Failed to parse WpForms responses file");
        return null;
    }

    private bool ExportFormInfo(List<Form> formDefinitions)
    {
        _formInfoExporter.Initialize("form_info", formDefinitions);

        if (_formInfoExporter.Initialized)
            return _formInfoExporter.ExportData();

        Logger.Error("Failed to initialize form info exporter");
        return false;
    }

    private bool ExportChoiceFields(List<Form> formDefinitions)
    {
        _choiceFieldsExporter.Initialize("choice_fields", formDefinitions);

        if (_choiceFieldsExporter.Initialized)
            return _choiceFieldsExporter.ExportData();

        Logger.Error("Failed to initialize choice fields exporter");
        return false;
    }

    private bool ExportFieldDescriptions(List<Form> formDefinitions)
    {
        _fieldDescriptionsExporter.Initialize("fields", formDefinitions);

        if (_fieldDescriptionsExporter.Initialized)
            return _fieldDescriptionsExporter.ExportData();

        Logger.Error("Failed to initialize field description exporter");
        return false;
    }

    private bool ExportResponses( ResponseInfo responseInfo )
    {
        _responsesExporter.Initialize("responses", responseInfo);

        if (_responsesExporter.Initialized)
            return _responsesExporter.ExportData();

        Logger.Error("Failed to initialize submissions exporter");
        return false;
    }

}
