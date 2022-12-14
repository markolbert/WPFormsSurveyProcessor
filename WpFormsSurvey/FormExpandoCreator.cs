using System.Dynamic;
using J4JSoftware.Logging;
using WPFormsSurvey;

namespace WpFormsSurvey;

public class FormExpandoCreator : JsonExpandoCreator<List<FieldBase>>
{
    public FormExpandoCreator( 
        IJ4JLogger logger 
    )
        : base( logger )
    {
    }

    public override List<FieldBase>? Create( ExpandoObject container )
    {
        var retVal = new List<FieldBase>();

        if( !GetPropertyValue<ExpandoObject>( container, "Children", out var temp ) )
            return retVal;

        dynamic children = temp!;

        foreach( KeyValuePair<string, ExpandoObject> kvp in children )
        {
            if( CreateField( kvp.Value, out var newField ) )
                retVal.Add( newField! );
        }

        return retVal;
    }

    private bool CreateField(ExpandoObject fieldData, out FieldBase? result)
    {
        result = null;

        if (!GetPropertyValue<string>(fieldData, "type", out var fieldType ))
            return false;

        if (string.IsNullOrEmpty(fieldType))
            return false;

        result = fieldType.ToLower() switch
        {
            "content" => CreateContentField(fieldData),
            "name" => CreateNameField(fieldData),
            "email" => CreateEmailField(fieldData),
            "radio" => CreateRadioField(fieldData),
            "text" => CreateTextField(fieldData),
            "number-slider" => CreateNumberSliderField(fieldData),
            "textarea" => CreateTextField(fieldData),
            "checkbox" => CreateCheckboxField(fieldData),
            _ => null
        };

        return result != null;
    }

    private ContentField CreateContentField(ExpandoObject container)
    {
        var retVal = InitializeField<ContentField>(container);

        retVal.Content = GetPropertyValue<string>(container, "content", out var tempContent)
            ? tempContent!
            : string.Empty;

        return retVal;
    }

    private NameField CreateNameField(ExpandoObject container)
    {
        var retVal = InitializeField<NameField>(container);

        retVal.Format = GetPropertyValue<string>(container, "format", out var tempFormat)
            ? tempFormat!
            : string.Empty;

        return retVal;
    }

    private EmailField CreateEmailField(ExpandoObject container) => InitializeField<EmailField>(container);

    private RadioField CreateRadioField(ExpandoObject container)
    {
        var retVal = InitializeField<RadioField>(container);

        if (!GetPropertyValue<List<ExpandoObject>>(container, "choices", out var tempList))
            return retVal;

        foreach (var choiceObj in tempList!)
        {
            retVal.Choices.Add(new FieldChoice
            {
                Label =
                    GetPropertyValue<string>(choiceObj, "label", out var tempLabel)
                        ? tempLabel!
                        : string.Empty,
                Value = GetPropertyValue<string>(choiceObj, "value", out var tempValue)
                    ? tempValue!
                    : string.Empty,
                Image = GetPropertyValue<string>(choiceObj, "image", out var tempImage)
                    ? tempImage!
                    : string.Empty
            });
        }

        return retVal;
    }

    private CheckboxField CreateCheckboxField(ExpandoObject container)
    {
        var retVal = InitializeField<CheckboxField>(container);

        if (!GetPropertyValue<List<ExpandoObject>>(container, "choices", out var tempList))
            return retVal;

        foreach (var choiceObj in tempList!)
        {
            retVal.Choices.Add(new FieldChoice
            {
                Label =
                    GetPropertyValue<string>(choiceObj, "label", out var tempLabel)
                        ? tempLabel!
                        : string.Empty,
                Value = GetPropertyValue<string>(choiceObj, "value", out var tempValue)
                    ? tempValue!
                    : string.Empty,
                Image = GetPropertyValue<string>(choiceObj, "image", out var tempImage)
                    ? tempImage!
                    : string.Empty
            });
        }

        return retVal;
    }

    private TextField CreateTextField(ExpandoObject container)
    {
        var retVal = InitializeField<TextField>(container);

        retVal.Label = GetPropertyValue<string>(container, "label", out var tempLabel) ? tempLabel! : string.Empty;

        return retVal;
    }

    private NumberSliderField CreateNumberSliderField(ExpandoObject container)
    {
        var retVal = InitializeField<NumberSliderField>(container);

        retVal.Label = GetPropertyValue<string>(container, "label", out var tempLabel) ? tempLabel! : string.Empty;

        var max = GetPropertyValue<string>(container, "max", out var tempMax) ? tempMax! : string.Empty;

        if (int.TryParse(max, out int maxVal))
            retVal.Maximum = maxVal;

        var min = GetPropertyValue<string>(container, "min", out var tempMin) ? tempMin! : string.Empty;

        if (int.TryParse(min, out int minVal))
            retVal.Minimum = minVal;

        var step = GetPropertyValue<string>(container, "step", out var tempStep) ? tempStep! : string.Empty;

        if (int.TryParse(step, out int stepVal))
            retVal.Step = stepVal;

        return retVal;
    }

    private TField InitializeField<TField>(ExpandoObject container)
        where TField : FieldBase, new()
    {
        var retVal = new TField()
        {
            Id = GetPropertyValue<int>(container, "id", out var tempId) ? tempId : 0
        };

        if (!GetPropertyValue<List<List<ExpandoObject>>>(container, "conditionals", out var tempList)
        || !tempList!.Any())
            return retVal;

        var innerList = tempList![0];
        if (!innerList.Any())
            return retVal;

        var conditionalData = innerList[0];

        retVal.Conditionals.Add(new FieldConditional
        {
            LinkedChoiceId = GetPropertyValue<int>(conditionalData, "value", out var tempValue) ? tempValue : 0,
            Operator = GetPropertyValue<string>(conditionalData, "operator", out var tempOperator) ? tempOperator! : string.Empty,
            LinkedFieldId = GetPropertyValue<int>(conditionalData, "field", out var tempField) ? tempField : 0
        });

        return retVal;
    }
}
