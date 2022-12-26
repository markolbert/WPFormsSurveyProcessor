﻿using J4JSoftware.Logging;
using NPOI.XSSF.UserModel;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal class ParseService : ServiceBase
{
    private readonly WpFormsParser _formsParser;
    private readonly WpResponsesParser _responsesParser;
    private readonly ExportFormInfo _formInfoExporter;
    private readonly ExportChoiceFields _choiceFieldsExporter;
    private readonly ExportFieldDescriptions _fieldDescriptionsExporter;
    private readonly ExportSubmissions _submissionsExporter;

    public ParseService(
        Configuration config,
        WpFormsParser formsParser,
        WpResponsesParser responsesParser,
        ExportFormInfo formInfoExporter,
        ExportChoiceFields choiceFieldsExporter,
        ExportFieldDescriptions fieldDescriptionsExporter,
        ExportSubmissions submissionsExporter,
        IJ4JLogger logger
    )
        : base(config, logger)
    {
        _formsParser = formsParser;
        _responsesParser = responsesParser;

        _formInfoExporter = formInfoExporter;
        _choiceFieldsExporter = choiceFieldsExporter;
        _fieldDescriptionsExporter = fieldDescriptionsExporter;
        _submissionsExporter = submissionsExporter;
    }

    public override Task StartAsync( CancellationToken cancellationToken )
    {
        if( !Configuration.ExcelFileInfo.CanBeWritten )
        {
            Logger.Error("Cannot create or write to the Excel file path");
            return Task.CompletedTask;
        }

        var formsDownload = ParseForms();
        if( formsDownload?.Data is not {} formDefinitions )
            return Task.CompletedTask;

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
        if( responsesDownload?.Data is not {} individualSubmissions )
            return Task.CompletedTask;

        if( Configuration.FormIds.Any() )
        {
            individualSubmissions = individualSubmissions
                                   .Where( x => Configuration.FormIds.Any( y => x.FormId == y ) )
                                   .ToList();

            if( !individualSubmissions.Any() )
            {
                Logger.Warning( "No submissions(s) with that/those form ids were found" );
                return Task.CompletedTask;
            }
        }

        var workbook = new XSSFWorkbook( XSSFWorkbookType.XLSX );

        if( ( Configuration.ExcelFileInfo.InformationToExport & Exporters.FormInformation ) == Exporters.FormInformation
        && !ExportFormInfo( workbook, formDefinitions ) )
            return Task.CompletedTask;

        if( ( Configuration.ExcelFileInfo.InformationToExport & Exporters.FieldDescriptions ) == Exporters.FieldDescriptions
        && !ExportFieldDescriptions( workbook, formDefinitions ) )
            return Task.CompletedTask;

        if( ( Configuration.ExcelFileInfo.InformationToExport & Exporters.ChoiceFields ) == Exporters.ChoiceFields
        && !ExportChoiceFields( workbook, formDefinitions ) )
            return Task.CompletedTask;

        if( ( Configuration.ExcelFileInfo.InformationToExport & Exporters.Submissions ) == Exporters.Submissions
        && !ExportSubmissions( workbook, new SubmissionInfo( formDefinitions, individualSubmissions ) ) )
            return Task.CompletedTask;

        try
        {
            using var fileStream = File.Create( Configuration.ExcelFileInfo
                                                             .GetTimeStampedPath( Configuration.EntriesFilePath ) );

            Logger.Information<string>( "Writing results to {0}", fileStream.Name );

            workbook.Write( fileStream );
            fileStream.Close();
        }
        catch( IOException ioException )
        {
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

    private bool ExportFormInfo(XSSFWorkbook workbook, List<Form> formDefinitions)
    {
        _formInfoExporter.Initialize(workbook, "form_info", formDefinitions);

        if (_formInfoExporter.Initialized)
            return _formInfoExporter.ExportData();

        Logger.Error("Failed to initialize form info exporter");
        return false;
    }

    private bool ExportChoiceFields(XSSFWorkbook workbook, List<Form> formDefinitions)
    {
        _choiceFieldsExporter.Initialize(workbook, "choice_fields", formDefinitions);
        if (_choiceFieldsExporter.Initialized)
            return _choiceFieldsExporter.ExportData();

        Logger.Error("Failed to initialize choice fields exporter");
        return false;
    }

    private bool ExportFieldDescriptions(XSSFWorkbook workbook, List<Form> formDefinitions)
    {
        _fieldDescriptionsExporter.Initialize(workbook, "fields", formDefinitions);
        if (_fieldDescriptionsExporter.Initialized)
            return _fieldDescriptionsExporter.ExportData();

        Logger.Error("Failed to initialize field description exporter");
        return false;
    }

    private bool ExportSubmissions( XSSFWorkbook workbook, SubmissionInfo submissionInfo )
    {
        _submissionsExporter.Initialize(workbook, "responses", submissionInfo);
        if (_submissionsExporter.Initialized)
            return _submissionsExporter.ExportData();

        Logger.Error("Failed to initialize submissions exporter");
        return false;
    }

}
