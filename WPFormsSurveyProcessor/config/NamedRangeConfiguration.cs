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

using J4JSoftware.DeusEx;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.UserModel;
using System.ComponentModel;
using System.Text;

namespace J4JSoftware.WpFormsSurvey;

public class NamedRangeConfiguration
{
    private readonly IJ4JLogger _logger;

    private string? _lastCol;

    public NamedRangeConfiguration()
    {
        _logger = J4JDeusEx.ServiceProvider.GetRequiredService<IJ4JLogger>();
        _logger.SetLoggedType( GetType() );
    }

    public string Name { get; set; } = string.Empty;
    public NamedRangeContext Context { get; set; } = NamedRangeContext.Workbook;
    public string FirstColumn { get; set; } = string.Empty;
    public bool IncludeHeader { get; set; }

    public string LastColumn
    {
        get => string.IsNullOrEmpty( _lastCol ) ? FirstColumn : _lastCol;
        set => _lastCol = value;
    }

    public bool IsValid()
    {
        var retVal = true;

        if( string.IsNullOrEmpty( Name ) )
        {
            retVal = false;
            _logger.Error( "Named range has no name" );
        }

        if( string.IsNullOrEmpty( FirstColumn ) )
        {
            retVal = false;
            _logger.Error<string>( "First column of named range '{0}' is empty or undefined", Name );
        }

        if( string.IsNullOrEmpty( LastColumn ) )
        {
            retVal = false;
            _logger.Error<string>( "Last column of named range '{0}' is empty or undefined", Name );
        }

        if( FirstColumn.Any( x => !char.IsLetter( x ) ) )
        {
            retVal = false;
            _logger.Error<string>( "First column of named range '{0}' contains invalid characters", Name );
        }

        if( LastColumn.Any( x => !char.IsLetter( x ) ) )
        {
            retVal = false;
            _logger.Error<string>( "Last column of named range '{0}' contains invalid characters", Name );
        }

        var firstIndex = FirstColumn.Aggregate( 0.0, ( d, c ) => 256 * d + char.GetNumericValue( c ) );
        var lastIndex = LastColumn.Aggregate( 0.0, ( d, c ) => 256 * d + char.GetNumericValue( c ) );

        if( !( firstIndex > lastIndex ) )
            return retVal;

        retVal = false;
        _logger.Error<string>( "Last column of named range '{0}' precedes first column", Name );

        return retVal;
    }

    public string GetRangeName( IWorkbook workbook, ISheet worksheet )
    {
        var junk = workbook.GetAllNames();

        var sheetIndex = Context switch
        {
            NamedRangeContext.Workbook => -1,
            NamedRangeContext.Worksheet => workbook.GetSheetIndex( worksheet ),
            _ => throw new InvalidEnumArgumentException( $"Unsupported {nameof( NamedRangeContext )} '{Context}'" )
        };

        var nameSuffix = 1;

        do
        {
            if( !workbook.GetNames( $"{Name}{NumToText()}" ).Any() )
                break;

            nameSuffix++;
        } while( true );

        return $"{Name}{NumToText()}";

        string NumToText() => nameSuffix <= 1 ? string.Empty : nameSuffix.ToString();
    }

    public string GetRangeFormula( ISheet worksheet, int height )
    {
        var sb = new StringBuilder();

        if( Context == NamedRangeContext.Worksheet )
            sb.Append( $"{worksheet.SheetName}!" );

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if( IncludeHeader )
            sb.Append( $"${FirstColumn}$1:${LastColumn}${height}" );
        else sb.Append( $"${FirstColumn}$2:${LastColumn}${height}" );

        return sb.ToString();
    }

    public int GetRangeSheetIndex( IWorkbook workbook, ISheet worksheet ) =>
        Context switch
        {
            NamedRangeContext.Workbook => -1,
            NamedRangeContext.Worksheet => workbook.GetSheetIndex( worksheet ),
            _ => throw new InvalidEnumArgumentException( $"Unsupported {nameof( NamedRangeContext )} '{Context}'" )
        };
}