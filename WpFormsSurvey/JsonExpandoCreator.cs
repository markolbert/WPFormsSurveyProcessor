using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Logging;
using Serilog.Core;

namespace WpFormsSurvey;

public abstract class JsonExpandoCreator<TTarget> : IJsonDynamicCreator<TTarget>
{
    protected JsonExpandoCreator (
        IJ4JLogger logger
    )
    {
        Logger = logger;
        Logger.SetLoggedType( GetType() );
    }

    protected IJ4JLogger Logger { get; }

    public abstract TTarget? Create( ExpandoObject container );

    protected bool GetPropertyValue<TProp>(ExpandoObject container, string name, out TProp? result )
    {
        result = default(TProp);

        if (((IDictionary<string, object?>)container).TryGetValue(name, out var temp))
        {
            if (temp is TProp temp2)
            {
                result = temp2;
                return true;
            }

            Logger?.Error<string, Type>("Property '{0}' is not a {1}", name, typeof(TProp));
        }
        else Logger?.Error<string>("ExpandoObject does not contain '{0}'", name);

        return false;
    }
}