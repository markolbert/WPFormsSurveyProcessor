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

using NPOI.SS.UserModel;
using System.Text;
using J4JSoftware.Logging;

namespace J4JSoftware.WpFormsSurvey;

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
