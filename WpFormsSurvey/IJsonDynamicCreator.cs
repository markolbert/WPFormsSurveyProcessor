using System.Dynamic;

namespace WpFormsSurvey;

public interface IJsonDynamicCreator<out TTarget>
{
    TTarget? Create( ExpandoObject container );
}
