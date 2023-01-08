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
using NPOI.XSSF.UserModel;

namespace J4JSoftware.WpFormsSurvey;

internal class ExportChoiceFields : ExportSurveyBase<ChoiceFieldInfo>
{
    private List<Form>? _forms;

    public ExportChoiceFields( 
        Configuration config,
        IJ4JLogger logger )
        : base( config, logger )
    {
    }

    public bool Initialize( XSSFWorkbook workbook, string sheetName, List<Form> forms )
    {
        if( !Initialize(workbook, sheetName))
            return false;

        _forms = forms;

        Logger.Information("Beginning export of choice field information");
        return true;
    }

    public override bool Initialized => base.Initialized && _forms != null;

    protected override void ReportProgress() =>
        Logger.Information("    ...exported details on {0:n0} choice fields", RecordNumber);

    protected override bool StartExport()
    {
        SetCellValue("Form Id", null, 0);
        SetCellValue("Field Id");
        SetCellValue("Choice Id");
        SetCellValue("Field Key");
        SetCellValue("Choice Key");
        SetCellValue("Choice");
        MoveRows();

        return true;
    }

    protected override bool ProcessRecord( ChoiceFieldInfo entity )
    {
        SetCellValue(entity.FormId, null, 0);
        SetCellValue(entity.FieldId);
        SetCellValue(entity.ChoiceId);
        SetCellValue($"{entity.FormId}:{entity.FieldId}");
        SetCellValue($"{entity.FormId}:{entity.FieldId}:{entity.ChoiceId}");
        SetCellValue(entity.Text);
        MoveRows();

        return true;
    }

    protected override bool FinishExport()
    {
        AutoSizeColumns();

        if (RangeConfigurations?.Choices != null)
            Worksheet!.CreateWorksheetNamedRanges(RangeConfigurations.Choices,
                                                  RecordNumber + 1,
                                                  out _,
                                                  Logger);
        else Logger.Verbose("No named ranges defined");

        Logger.Information("    ...done");
        return true;
    }

    protected override IEnumerable<ChoiceFieldInfo> GetRecords()
    {
        if (!Initialized)
        {
            Logger.Error("Choice fields exporter is not initialized");
            yield break;
        }

        foreach( var curForm in _forms! )
        {
            foreach( var curField in curForm.Fields )
            {
                if( curField is not ChoicesField choiceField )
                    continue;

                var choiceId = 1;

                foreach( var choice in choiceField.Choices )
                {
                    yield return new ChoiceFieldInfo( curForm.Id, curField.Id, choiceId, choice.Label );

                    choiceId++;
                }
            }
        }
    }
}
