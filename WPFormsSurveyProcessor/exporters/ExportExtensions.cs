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

namespace J4JSoftware.WpFormsSurvey;

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
