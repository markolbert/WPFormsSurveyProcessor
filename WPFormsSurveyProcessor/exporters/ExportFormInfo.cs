using J4JSoftware.Logging;
using NPOI.XSSF.UserModel;

namespace J4JSoftware.WpFormsSurvey;

internal class ExportFormInfo : ExportSurveyBase<FormInfo>
{
    private List<Form>? _forms;

    public ExportFormInfo( 
        Configuration config,
        IJ4JLogger logger )
        : base( config, logger )
    {
    }

    public bool Initialize( XSSFWorkbook workbook, string sheetName, List<Form> forms )
    {
        if( !Initialize( workbook, sheetName ) )
            return false;

        _forms = forms;

        Logger.Information("Beginning export of form summary information");
        return true;
    }

    public override bool Initialized => base.Initialized && _forms != null;

    protected override void ReportProgress() =>
        Logger.Information("    ...exported information for {0:n0} forms", RecordNumber);

    protected override bool StartExport()
    {
        SetCellValue("Form Id", null, 0);
        SetCellValue("Form Name");
        MoveRows();

        return true;
    }

    protected override bool ProcessRecord( FormInfo entity )
    {
        SetCellValue(entity.Id, null, 0);
        SetCellValue(entity.Name);
        MoveRows();

        return true;
    }

    protected override bool FinishExport()
    {
        AutoSizeColumns();

        if( RangeConfigurations?.Forms != null )
            Worksheet!.CreateWorksheetNamedRanges( RangeConfigurations.Forms,
                                                   RecordNumber + 1,
                                                   out _,
                                                   Logger );

        Logger.Information("    ...done");
        return true;
    }

    protected override IEnumerable<FormInfo> GetRecords()
    {
        if( !Initialized )
        {
            Logger.Error("Form info exporter is not initialized");
            yield break;
        }

        foreach( var form in _forms! )
        {
            yield return new FormInfo( form.Id, form.PostTitle );
        }
    }
}
