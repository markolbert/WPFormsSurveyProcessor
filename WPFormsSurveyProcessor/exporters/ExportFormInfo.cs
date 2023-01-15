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

internal class ExportFormInfo : ExportSurveyBase<FormInfo>
{
    private List<Form>? _forms;

    public ExportFormInfo( 
        Configuration config,
        IJ4JLogger logger )
        : base( config, SheetType.Forms, logger )
    {
    }

    public bool Initialize( string sheetName, List<Form> forms )
    {
        if( !Initialize( sheetName ) )
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
        CreateNamedRanges(out _);

        Logger.Information( "    ...done" );
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
