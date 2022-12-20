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
                SetCellValue(likertResponseInfo.Response);
                SetCellValue(likertResponseInfo.Response);

                break;

            case UserFieldResponse<string> textResponse:
                MoveColumns(2);
                SetCellValue(textResponse.Response);
                break;

            case UserFieldResponse<DateTime> dtResponse:
                MoveColumns(2);
                SetCellValue(dtResponse.Response);

                break;

            case UserFieldResponse<decimal> numResponse:
                MoveColumns(2);
                SetCellValue(numResponse.Response);

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
                var curField = curForm.Fields
                                       .FirstOrDefault( x => x.Id == curResponse.FieldId );

                if( curField == null )
                {
                    Logger.Error( "Could not locate field {0} in form {1}", curResponse.FieldId, curForm.Id );
                    continue;
                }

                var enumerator = curField switch
                {
                    ChoicesField choicesField => ProcessChoicesField( submission, choicesField, curResponse ),
                    LikertField likertField => curResponse is LikertResponse likertResponse
                        ? ProcessLikertField( submission, likertField, likertResponse )
                        : null,
                    FormattedField formattedField => ProcessFormattedField( submission, formattedField, curResponse ),
                    NumberSliderField numSliderField => ProcessNumberSliderField(submission, numSliderField, curResponse ),
                    FileUploadField fileUploadField => ProcessFileUploadField(submission, fileUploadField, curResponse ),
                    TextField textField => ProcessTextField(submission, textField, curResponse ),
                    _ => null
                };

                if( enumerator == null )
                {
                    Logger.Error( "Could not process user {0} response for {1} field {2} in form {3} (id {4})",
                                  new object[]
                                  {
                                      submission.UserId,
                                      curField.GetType(),
                                      curResponse.FieldId,
                                      curForm.Id,
                                      curForm.PostTitle
                                  } );
                    continue;
                }

                foreach( var response in enumerator )
                {
                    if( response == null )
                        Logger.Error(
                            "Parsed response is null/undefined for user {0} response for {1} field {2} in form {3} (id {4})",
                            new object[]
                            {
                                submission.UserId,
                                curField.GetType(),
                                curResponse.FieldId,
                                curForm.Id,
                                curForm.PostTitle
                            } );
                    else yield return response;

                }
            }
        }
    }

    private IEnumerable<IUserFieldResponse?> ProcessChoicesField(
        IndividualSubmission submission,
        ChoicesField choicesField,
        ResponseBase curResponse
    )
    {
        var responses = new List<string>();

        switch (curResponse)
        {
            case MultipleTextResponse multipleTextResponse:
                responses.AddRange(multipleTextResponse.Values);
                break;

            case TextResponse textResponse:
                responses.Add(textResponse.Value);
                break;
        }

        foreach (var response in responses)
        {
            var responseInfo = new IndexedResponseInfo( curResponse.FieldLabel,
                                                        choicesField.Choices.FindIndex(
                                                            x => x.Label.Equals(
                                                                response,
                                                                StringComparison.OrdinalIgnoreCase ) ) );

            yield return new UserFieldResponse<IndexedResponseInfo>( submission.UserId,
                                                                     submission.FormId,
                                                                     submission.IpAddress,
                                                                     submission.Date,
                                                                     curResponse.FieldId,
                                                                     responseInfo );
        }
    }

    private IEnumerable<IUserFieldResponse?> ProcessLikertField(
        IndividualSubmission submission,
        LikertField likertField,
        LikertResponse likertResponse
    )
    {
        foreach( var likertScore in likertResponse.Scores )
        {
            var responseInfo = new LikertResponseInfo( likertScore.Column,
                                                       likertField.Columns[ likertScore.Column - 1 ],
                                                       likertScore.Column );

            yield return new UserFieldResponse<LikertResponseInfo>( submission.UserId,
                                                                    submission.FormId,
                                                                    submission.IpAddress,
                                                                    submission.Date,
                                                                    likertField.Id,
                                                                    responseInfo );
        }
    }

    private IEnumerable<IUserFieldResponse?> ProcessFormattedField(
        IndividualSubmission submission,
        FormattedField formattedField,
        ResponseBase curResponse
    )
    {
        yield return curResponse switch
        {
            NameResponse nameResponse => new UserFieldResponse<NameResponseInfo>( submission.UserId,
                submission.FormId,
                submission.IpAddress,
                submission.Date,
                formattedField.Id,
                new NameResponseInfo( nameResponse ) ),
            TextResponse textResponse => new UserFieldResponse<string>( submission.UserId,
                                                                        submission.FormId,
                                                                        submission.IpAddress,
                                                                        submission.Date,
                                                                        formattedField.Id,
                                                                        textResponse.Value ),
            _ => null
        };
    }

    private IEnumerable<IUserFieldResponse?> ProcessTextField(
        IndividualSubmission submission,
        TextField textField,
        ResponseBase curResponse
    )
    {
        yield return curResponse switch
        {
            TextResponse textResponse => new UserFieldResponse<string>(submission.UserId,
                                                                       submission.FormId,
                                                                       submission.IpAddress,
                                                                       submission.Date,
                                                                       textField.Id,
                                                                       textResponse.Value),
            _ => null
        };
    }

    private IEnumerable<IUserFieldResponse?> ProcessNumberSliderField(
        IndividualSubmission submission,
        NumberSliderField numSliderField,
        ResponseBase curResponse
    )
    {
        yield return curResponse switch
        {
            NumericResponse numResponse => new UserFieldResponse<decimal>(submission.UserId,
                                                                       submission.FormId,
                                                                       submission.IpAddress,
                                                                       submission.Date,
                                                                       numSliderField.Id,
                                                                       numResponse.Value),
            _ => null
        };
    }

    private IEnumerable<IUserFieldResponse?> ProcessFileUploadField(
        IndividualSubmission submission,
        FileUploadField numSliderField,
        ResponseBase curResponse
    )
    {
        yield return curResponse switch
        {
            TextResponse textResponse => new UserFieldResponse<string>(submission.UserId,
                                                                          submission.FormId,
                                                                          submission.IpAddress,
                                                                          submission.Date,
                                                                          numSliderField.Id,
                                                                          textResponse.Value),
            _ => null
        };
    }
}
