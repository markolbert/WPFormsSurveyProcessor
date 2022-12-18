using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Logging;
using NPOI.SS.UserModel;
using WpFormsSurvey;

namespace WPFormsSurveyProcessor;

internal class ExportFormInfo : ExportBase<FormInfo>
{
    private Forms? _forms;

    public ExportFormInfo( 
        IJ4JLogger logger )
        : base( logger )
    {
    }

    public void Initialize( IWorkbook workbook, Forms forms )
    {
        Initialize(workbook, "form_info");
        _forms = forms;
    }

    protected override bool ExportHeader()
    {
        SetCellValue("Form Id", null, 0);
        SetCellValue("Form Name");
        MoveRows();

        return true;
    }

    protected override bool ProcessEntity( FormInfo entity )
    {
        SetCellValue(entity.Id, null, 0);
        SetCellValue(entity.Name);
        MoveRows();

        return true;
    }

    protected override bool ExportFooter()
    {
        AutoSizeColumns();

        return true;
    }

    protected override IEnumerable<FormInfo> GetEntities()
    {
        foreach( var formInfo in _forms!.SummaryInfo )
        {
            yield return formInfo;
        }
    }
}
