// Copyright (c) 2022 Mark A. Olbert 
// all rights reserved
// This file is part of WpFormsSurvey.
//
// WpFormsSurvey is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// WpFormsSurvey is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with WpFormsSurvey. If not, see <https://www.gnu.org/licenses/>.

namespace J4JSoftware.WpFormsSurvey;

public record FormInfo( int Id, string Name )
{
    private sealed class IdEqualityComparer : IEqualityComparer<FormInfo>
    {
        public bool Equals( FormInfo? x, FormInfo? y )
        {
            if( ReferenceEquals( x, y ) )
                return true;
            if( ReferenceEquals( x, null ) )
                return false;
            if( ReferenceEquals( y, null ) )
                return false;
            if( x.GetType() != y.GetType() )
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode( FormInfo obj )
        {
            return obj.Id;
        }
    }

    public static IEqualityComparer<FormInfo> DefaultComparer { get; } = new IdEqualityComparer();
}
