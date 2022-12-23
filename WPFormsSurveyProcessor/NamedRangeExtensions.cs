using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Logging;
using System.Xml.Linq;

namespace WPFormsSurveyProcessor;

internal static class NamedRangeExtensions
{
    public static bool CreateWorksheetNamedRange(
        this ISheet sheet,
        NamedRangeConfiguration namedRange,
        int height,
        out IName? result,
        IJ4JLogger? logger = null
    ) =>
        sheet.CreateNamedRange(namedRange, height, sheet.Workbook.GetSheetIndex(sheet), out result, logger);

    public static bool CreateWorksheetNamedRanges(
        this ISheet sheet,
        IEnumerable<NamedRangeConfiguration> namedRanges,
        int height,
        out List<IName> result,
        IJ4JLogger? logger = null
    )
    {
        result = new List<IName>();
        var retVal = true;

        foreach( var namedRange in namedRanges )
        {
            if( sheet.CreateWorksheetNamedRange(namedRange,height, out var innerResult, logger ))
               result.Add(innerResult!  );
            else retVal = false;
        }

        return retVal;
    }

    public static bool CreateWorkbookNamedRange(
        this ISheet sheet,
        NamedRangeConfiguration namedRange,
        int height,
        out IName? result,
        IJ4JLogger? logger = null
    ) =>
        sheet.CreateNamedRange(namedRange, height, -1, out result);

    public static bool CreateWorkbookNamedRanges(
        this ISheet sheet,
        IEnumerable<NamedRangeConfiguration> namedRanges,
        int height,
        out List<IName> result,
        IJ4JLogger? logger = null
    )
    {
        result = new List<IName>();
        var retVal = true;

        foreach (var namedRange in namedRanges)
        {
            if (sheet.CreateWorkbookNamedRange(namedRange, height, out var innerResult, logger))
                result.Add(innerResult!);
            else retVal = false;
        }

        return retVal;
    }

    public static bool CreateNamedRange(
        this ISheet sheet,
        NamedRangeConfiguration namedRange,
        int height,
        int sheetIndex,
        out IName? result,
        IJ4JLogger? logger = null
    )
    {
        result = null;

        if (!namedRange.IsValid)
        {
            foreach (var error in namedRange.Errors)
            {
                logger?.Error(error);
            }

            return false;
        }

        var minHeight = namedRange.IncludeHeader ? 1 : 2;
        if (height < minHeight)
        {
            logger?.Error("Range height cannot be less than {0}", minHeight);
            return false;
        }

        var sb = new StringBuilder();

        if( namedRange.Context == NamedRangeContext.Worksheet )
            sb.Append( $"{sheet.SheetName}!" );

        if( namedRange.IncludeHeader )
            sb.Append( $"${namedRange.FirstColumn}$1:${namedRange.LastColumn}${height}" );
        else sb.Append( $"${namedRange.FirstColumn}$2:${namedRange.LastColumn}${height}" );

        var workbook = sheet.Workbook;

        foreach (var existingRange in workbook.GetNames(namedRange.Name))
        {
            if (existingRange.SheetIndex != sheetIndex)
                continue;

            logger?.Error<string, string>( $"Named range {0}!{1} already exists", sheet.SheetName, namedRange.Name );
            return false;
        }

        try
        {
            result = workbook.CreateName();
            result.NameName = namedRange.Name;
            result.RefersToFormula = sb.ToString();

            if (sheetIndex >= 0)
                result.SheetIndex = sheetIndex;
        }
        catch (Exception ex)
        {
            logger?.Error<string, string>("Failed to create named range '{0}', message was {1}", namedRange.Name, ex.Message);
            result = null;
        }

        return result != null;
    }
}
