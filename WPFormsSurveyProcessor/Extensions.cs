using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.WpFormsSurvey;
internal static class Extensions
{
    // thanx to RobinHood70 for this
    // https://stackoverflow.com/questions/4171140/how-to-iterate-over-values-of-an-enum-having-flags
    public static IEnumerable<T> GetUniqueFlags<T>(this T value)
        where T : Enum
    {
        var valueLong = Convert.ToUInt64(value, CultureInfo.InvariantCulture);

        foreach( var enumValue in value.GetType().GetEnumValues() )
        {
            if( enumValue is T flag // cast enumValue to T
            && Convert.ToUInt64( flag, CultureInfo.InvariantCulture ) is var bitValue // convert flag to ulong
            && ( bitValue & ( bitValue - 1 ) ) == 0 // is this a single-bit value?
            && ( valueLong & bitValue ) != 0 // is the bit set?
              )
                yield return flag;
        }
    }
}
