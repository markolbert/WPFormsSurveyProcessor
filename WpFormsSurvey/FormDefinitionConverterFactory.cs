using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using J4JSoftware.Logging;
using WPFormsSurvey;

namespace WpFormsSurvey;

public class FormDefinitionConverterFactory : JsonConverterFactory
{
    private readonly IJ4JLogger _logger;

    public FormDefinitionConverterFactory(
        IJ4JLogger logger
        )
    {
        _logger = logger;
        _logger.SetLoggedType( GetType() );
    }

    public override bool CanConvert( Type typeToConvert )
    {
        if( !typeToConvert.IsGenericType )
        {
            _logger.Error( "Trying to convert a non-generic type" );
            return false;
        }

        if( typeToConvert.GetGenericTypeDefinition() != typeof( List<> ) )
        {
            _logger.Error( "Trying to convert a non-List type" );
            return false;
        }

        var retVal = typeToConvert.GetGenericArguments()[ 0 ]
                                  .IsAssignableTo( typeof( FieldBase ) );

        if( !retVal )
            _logger.Error( "Trying to convert to something other than a {0}", typeof( List<FieldBase> ) );

        return retVal;
    }

    public override JsonConverter? CreateConverter( Type typeToConvert, JsonSerializerOptions options )
    {

        throw new NotImplementedException();
    }
}