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

using J4JSoftware.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace J4JSoftware.WpFormsSurvey;

internal class ExportFieldDescriptions : ExportSurveyBase<FieldDescription>
{
    private List<Form>? _forms;

    public ExportFieldDescriptions(
        Configuration config,
        IJ4JLogger logger )
        : base( config, SheetType.Fields, logger )
    {
    }

    public bool Initialize(string sheetName, List<Form> forms)
    {
        if (!Initialize(sheetName))
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
        CreateNamedRanges(out _);

        Logger.Information( "    ...done" );
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
