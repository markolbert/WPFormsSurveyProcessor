using System.Text;
using J4JSoftware.Logging;
using NPOI.SS.UserModel;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal class ExportSubmissions : ExportBase<IUserFieldResponse>
{
    private SubmissionInfo? _submissionInfo;

    public ExportSubmissions( 
        IJ4JLogger logger )
        : base( logger )
    {
    }

    public bool Initialize( IWorkbook workbook, string sheetName, SubmissionInfo dataSource )
    {
        if( !Initialize( workbook, sheetName ) )
            return false;

        _submissionInfo = dataSource;

        Logger.Information("Beginning export of form submissions");
        return true;
    }

    public override bool Initialized => base.Initialized && _submissionInfo != null;

    protected override void ReportProgress() =>
        Logger.Information( "    ...exported {0:n0} submissions", RecordNumber );

    protected override bool StartExport()
    {
        SetCellValue("User Id", null, 0);
        SetCellValue("IP Address");
        SetCellValue("Date");
        SetCellValue("Form Id");
        SetCellValue("Field Id");
        SetCellValue("Field Type");
        SetCellValue("Subfield Id");
        SetCellValue("Response Index");
        SetCellValue("Response");

        MoveRows();

        return true;
    }

    protected override bool ProcessRecord( IUserFieldResponse entity )
    {
        SetCellValue(entity.UserId, null, 0);
        SetCellValue(entity.IpAddress);
        SetCellValue(entity.Submitted);
        SetCellValue(entity.FormId);
        SetCellValue(entity.FieldId);
        SetCellValue(entity.FieldType);

        var response = entity.GetResponse();

        switch( response )
        {
            case IndexedResponseInfo indexedResponse:
                MoveColumns();
                SetCellValue(indexedResponse.ResponseIndex);
                SetCellValue(indexedResponse.Response);

                break;

            case NameResponseInfo nameResponseInfo:
                MoveColumns(2);
                SetCellValue(FormatName(nameResponseInfo));

                break;

            case LikertResponseInfo likertResponseInfo:
                SetCellValue(likertResponseInfo.SubFieldId);
                SetCellValue(likertResponseInfo.ResponseIndex);
                SetCellValue(likertResponseInfo.Response);

                break;

            case string textResponse:
                MoveColumns(2);
                SetCellValue(textResponse);
                break;

            case DateTime dtResponse:
                MoveColumns(2);
                SetCellValue(dtResponse);

                break;

            case decimal numResponse:
                MoveColumns(2);
                SetCellValue(numResponse);

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
