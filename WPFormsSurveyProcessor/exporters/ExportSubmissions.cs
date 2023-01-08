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

using System.Text;
using J4JSoftware.Logging;
using NPOI.SS.UserModel;

namespace J4JSoftware.WpFormsSurvey;

internal class ExportSubmissions : ExportSurveyBase<IUserFieldResponse>
{
    private SubmissionInfo? _submissionInfo;
    private int _users;
    private int _prevUserId;

    public ExportSubmissions( 
        Configuration config,
        IJ4JLogger logger )
        : base( config, logger )
    {
    }

    public bool Initialize( IWorkbook workbook, string sheetName, SubmissionInfo dataSource )
    {
        if( !Initialize( workbook, sheetName ) )
            return false;

        _submissionInfo = dataSource;

        Styles = new CustomStyles( Workbook! );
        var dtStyle = Workbook!.CreateCellStyle();
        dtStyle.DataFormat = Workbook!.CreateDataFormat().GetFormat( "M/d/yyyy h:mm:ss" );
        Styles!.Add( new CustomStyles.Style( "Date Submitted", dtStyle ) );

        Logger.Information("Beginning export of form submissions");
        return true;
    }

    public override bool Initialized => base.Initialized && _submissionInfo != null;

    protected override void ReportProgress() =>
        Logger.Information( "    ...exported {0:n0} responses from {1:n0} users", RecordNumber, _users );

    protected override bool StartExport()
    {
        SetCellValue("User Id", null, 0);
        SetCellValue("IP Address");
        SetCellValue("Date");
        SetCellValue("Form Id");
        SetCellValue("Field Id");
        SetCellValue("Field Type");
        SetCellValue("Subfield Id");
        SetCellValue("Field Key");
        SetCellValue("Subfield Key");
        SetCellValue("Response Index");
        SetCellValue("Response");

        MoveRows();

        _users = 0;
        _prevUserId = -1;

        return true;
    }

    protected override bool ProcessRecord( IUserFieldResponse entity )
    {
        if( entity.UserId != _prevUserId )
        {
            _users++;
            _prevUserId = entity.UserId;
        }

        SetCellValue(entity.UserId, null, 0);
        SetCellValue(entity.IpAddress);
        SetCellValue(entity.Submitted, "Date Submitted");
        SetCellValue(entity.FormId);
        SetCellValue(entity.FieldId);
        SetCellValue(entity.FieldType);

        var response = entity.GetResponse();

        switch( response )
        {
            case IndexedResponseInfo indexedResponse:
                SetCellValue( $"{entity.FormId}:{entity.FieldId}", null, 2);
                SetCellValue(indexedResponse.ResponseIndex, null, 2);
                SetCellValue(indexedResponse.Response);

                break;

            case NameResponseInfo nameResponseInfo:
                SetCellValue($"{entity.FormId}:{entity.FieldId}", null, 2);
                SetCellValue(FormatName(nameResponseInfo), null, 3);

                break;

            case LikertResponseInfo likertResponseInfo:
                SetCellValue(likertResponseInfo.SubFieldId);
                SetCellValue($"{entity.FormId}:{entity.FieldId}");
                SetCellValue($"{entity.FormId}:{entity.FieldId}:{likertResponseInfo.SubFieldId}");
                SetCellValue(likertResponseInfo.ResponseIndex);
                SetCellValue(likertResponseInfo.Response);

                break;

            case string textResponse:
                SetCellValue($"{entity.FormId}:{entity.FieldId}", null, 2);
                SetCellValue(textResponse, null, 3);
                break;

            case DateTime dtResponse:
                SetCellValue($"{entity.FormId}:{entity.FieldId}", null, 2);
                SetCellValue(dtResponse, null, 3);

                break;

            case decimal numResponse:
                SetCellValue($"{entity.FormId}:{entity.FieldId}", null, 2);
                SetCellValue(numResponse, null, 3);

                break;
        }

        MoveRows();

        return true;
    }

    private string FormatName( NameResponseInfo nameResponseInfo )
    {
        var sb = new StringBuilder();

        sb.Append( nameResponseInfo.FirstName );

        if( !string.IsNullOrEmpty( nameResponseInfo.MiddleName ) )
        {
            if( sb.Length > 0 )
                sb.Append( " " );

            sb.Append( nameResponseInfo.MiddleName );
        }

        if( string.IsNullOrEmpty( nameResponseInfo.LastName ) )
            return sb.ToString();

        if( sb.Length > 0 )
            sb.Append( " " );

        sb.Append( nameResponseInfo.LastName );

        return sb.ToString();
    }

    protected override bool FinishExport()
    {
        AutoSizeColumns();

        if( RangeConfigurations?.Submissions != null )
            Worksheet!.CreateWorksheetNamedRanges( RangeConfigurations.Submissions,
                                                   RecordNumber + 1,
                                                   out _,
                                                   Logger );
        else Logger.Verbose( "No named ranges defined" );

        Logger.Information("    ...done");
        return true;
    }

    protected override IEnumerable<IUserFieldResponse> GetRecords()
    {
        if( !Initialized )
        {
            Logger.Error("User submission exporter not initialized"  );
            yield break;
        }

        foreach( var submission in _submissionInfo!.UserSubmissions )
        {
            var curForm = _submissionInfo.Forms.FirstOrDefault( x => x.Id == submission.FormId );
            if( curForm == null )
            {
                Logger.Error( "Couldn't find form description for form id {0}", submission.FormId );
                continue;
            }

            foreach( var curResponse in submission.Responses )
            {
                var responseExport = curResponse.GetResponseExport( curForm, submission, curResponse.FieldId );

                if( !responseExport.IsValid )
                    Logger.Error( responseExport.Error! );

                foreach( var item in responseExport.Responses )
                {
                    yield return item;
                }
            }
        }
    }
}
