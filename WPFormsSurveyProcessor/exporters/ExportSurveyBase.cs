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

using System.ComponentModel;
using J4JSoftware.Logging;
using NPOI.SS.UserModel;

namespace J4JSoftware.WpFormsSurvey;

internal abstract class ExportSurveyBase<TEntity> : ExportBase<TEntity>
    where TEntity : class
{
    protected ExportSurveyBase( 
        Configuration config,
        SheetType sheetType,
        IJ4JLogger logger, 
        int reportingInterval = 500 
        )
        : base( logger, reportingInterval )
    {
        if( config.Workbook == null )
        {
            Logger.Fatal("Workbook is not configured");
            throw new ApplicationException( "Workbook is not configured" );
        }

        Configuration = config;
        SheetType = sheetType;

        var sheetInfo = Configuration.WorksheetDefinitions
                                     .FirstOrDefault( x => x.SheetType == sheetType );

        if( sheetInfo == null )
        {
            Logger.Fatal( "Could not find information for worksheet type '{0}'", sheetType );
            throw new NullReferenceException( $"Could not find information for worksheet type '{sheetType}'" );
        }

        RangeConfigurations = sheetInfo.Ranges;

        if( !RangeConfigurations.Any())
            Logger.Information($"No named ranges defined for sheet type {0}", sheetType);
    }

    protected Configuration Configuration { get; }
    protected SheetType SheetType { get; }
    protected override IWorkbook Workbook => Configuration.Workbook!;

    protected List<NamedRangeConfiguration> RangeConfigurations { get; }

    protected bool CreateNamedRanges( out List<IName> result )
    {
        result = new List<IName>();
        var retVal = true;

        foreach( var namedRange in RangeConfigurations )
        {
            if( CreateNamedRange( namedRange, out var temp ) )
                result.Add( temp! );
            else retVal = false;
        }

        return retVal;
    }

    private bool CreateNamedRange( NamedRangeConfiguration namedRange, out IName? result )
    {
        result = null;

        if (!namedRange.IsValid())
            return false;

        var rangeName = namedRange.GetRangeName( Workbook, Worksheet! );

        var height = RecordNumber + 1;
        var minHeight = namedRange.IncludeHeader ? 1 : 2;

        if (height < minHeight)
        {
            Logger.Error("Range height cannot be less than {0}", minHeight);
            return false;
        }

        var rangeFormula = namedRange.GetRangeFormula( Worksheet!, height );
        var rangeSheetIndex = namedRange.GetRangeSheetIndex( Workbook, Worksheet! );

        try
        {
            result = Workbook.CreateName();
            result.NameName = rangeName;
            result.RefersToFormula = rangeFormula;

            if (rangeSheetIndex >= 0)
                result.SheetIndex = rangeSheetIndex;
        }
        catch (Exception ex)
        {
            Logger.Error<string, string>("Failed to create named range '{0}', message was {1}", $"{rangeName}", ex.Message);
            result = null;
        }

        return result != null;
    }
}
