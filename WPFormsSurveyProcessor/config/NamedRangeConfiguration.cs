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

namespace J4JSoftware.WpFormsSurvey;

public class NamedRangeConfiguration
{
    private string? _lastCol;

    public string Name { get; set; } = string.Empty;
    public NamedRangeContext Context { get; set; } = NamedRangeContext.Workbook;
    public string FirstColumn { get; set; } = string.Empty;
    public bool IncludeHeader { get; set; }

    public string LastColumn
    {
        get => string.IsNullOrEmpty(_lastCol) ? FirstColumn : _lastCol;
        set => _lastCol = value;
    }

    public bool IsValid => !Errors.Any();

    public List<string> Errors
    {
        get
        {
            var retVal = new List<string>();

            if( string.IsNullOrEmpty( FirstColumn ) )
                retVal.Add( "First column is empty or undefined" );

            if( string.IsNullOrEmpty( LastColumn ) )
                retVal.Add( "Last column is empty or undefined" );

            if( FirstColumn.Any( x => !char.IsLetter( x ) ) )
                retVal.Add( "First column contains invalid characters" );

            if( LastColumn.Any( x => !char.IsLetter( x ) ) )
                retVal.Add( "Last column contains invalid characters" );

            var firstIndex = FirstColumn.Aggregate( 0.0, ( d, c ) => 256 * d + char.GetNumericValue( c ) );
            var lastIndex = LastColumn.Aggregate( 0.0, ( d, c ) => 256 * d + char.GetNumericValue( c ) );

            if( firstIndex > lastIndex )
                retVal.Add( "Last column precedes first column" );

            return retVal;
        }
    }
}