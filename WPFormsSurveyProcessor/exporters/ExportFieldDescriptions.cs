using J4JSoftware.Logging;
using NPOI.XSSF.UserModel;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal class ExportFieldDescriptions : ExportBase<FieldDescription>
{
    private List<Form>? _forms;

    public ExportFieldDescriptions( 
        IJ4JLogger logger )
        : base( logger )
    {
    }

    public bool Initialize(XSSFWorkbook workbook, string sheetName, List<Form> forms)
    {
        if (!Initialize(workbook, sheetName))
            return false;

        _forms = forms;

        Logger.Information("Beginning export of field descriptions");
        return true;
    }

    public override bool Initialized => base.Initialized && _forms != null;

    protected override void ReportProgress() =>
        Logger.Information("    ...exported {0:n0} field descriptions", RecordNumber);

    protected override bool StartExport()
    {
        SetCellValue("Form Id", null, 0);
        SetCellValue("Field Id");
        SetCellValue("Key");
        SetCellValue("Field Type");
        SetCellValue("Field Label");
        MoveRows();

        return true;
    }

    protected override bool ProcessRecord( FieldDescription entity )
    {
        SetCellValue(entity.FormId, null, 0);
        SetCellValue(entity.FieldId);
        SetCellValue($"{entity.FormId}:{entity.FieldId}");
        SetCellValue(entity.FieldType);
        SetCellValue(entity.Label);
        MoveRows();

        return true;
    }

    protected override bool FinishExport()
    {
        AutoSizeColumns();

        //CreateWorksheetNamedRange("FormIds", $"{SheetName}!$A$2:$A${RecordNumber + 1}", out _);
        //CreateWorksheetNamedRange("FieldIds", $"{SheetName}!$B$2:$B${RecordNumber + 1}", out _);
        CreateWorksheetNamedRange("Fields", $"{SheetName}!$C$2:$E${RecordNumber + 1}", out _);

        Logger.Information("    ...done");
        return true;
    }

    protected override IEnumerable<FieldDescription> GetRecords()
    {
        if (!Initialized)
        {
            Logger.Error("Field description exporter is not initialized");
            yield break;
        }

        foreach ( var curForm in _forms! )
        {
            foreach( var curField in curForm.Fields )
            {
                var label = curField is LabeledField labeledField ? labeledField.Label : string.Empty;

                yield return new FieldDescription( curForm.Id, curField.Id, curField.FieldType, label );
            }
        }
    }
}
