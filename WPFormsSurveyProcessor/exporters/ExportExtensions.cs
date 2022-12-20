using J4JSoftware.Logging;
using NPOI.SS.UserModel;

namespace WPFormsSurveyProcessor;

public static class ExportExtensions
{
    public static ISheet? CreateWorksheet( this IWorkbook workbook, string sheetName, bool replaceExisting = false, IJ4JLogger? logger = null )
    {
        var retVal = workbook.GetSheet(sheetName);

        if( retVal == null || !replaceExisting )
            return retVal;

        logger?.Warning<string>("Worksheet '{0}' already exists in workbook, removing it", sheetName);

        var idx = workbook.GetSheetIndex(retVal);
        workbook.RemoveSheetAt(idx);

        retVal = workbook.GetSheet(sheetName);

        return retVal;
    }
}
