using System.Reflection;
using System.Text.Json;
using J4JSoftware.Logging;

namespace WpFormsSurvey;

public class WpParserBase<TEntity>
    where TEntity : class
{
    protected record FieldDefinition(string Type, string FieldText);

    protected record EntityInfo(Type EntityType, MethodInfo DeserializerInfo);

    protected readonly Dictionary<string, EntityInfo> EntityTypes = new(StringComparer.OrdinalIgnoreCase);
    protected readonly MethodInfo? GenericDeserializer;
    protected IJ4JLogger Logger;

    protected WpParserBase(
        IJ4JLogger logger
    )
    {
        var thisType = GetType();

        Logger = logger;
        Logger.SetLoggedType(thisType);

        var temp = typeof(JsonSerializer)
                           .GetMethods()
                           .FirstOrDefault(x =>
                            {
                                if (!x.IsGenericMethod || x.GetGenericArguments().Length != 1)
                                    return false;

                                var args = x.GetParameters();

                                if (args.Length != 2)
                                    return false;

                                if (args[0].ParameterType != typeof(string))
                                    return false;

                                return args[1].ParameterType == typeof(JsonSerializerOptions);
                            });

        if (temp == null)
            throw new ApplicationException($"Could not find {nameof(JsonSerializer.Deserialize)}");

        GenericDeserializer = temp;

        RegisterEntityTypes(thisType.Assembly);
    }

    public bool RegisterEntityType<TField>() => RegisterEntityType(typeof(TField));

    public bool RegisterEntityType(Type entityType)
    {
        if (!entityType.IsAssignableTo(typeof(TEntity)))
        {
            Logger.Error("{0} does not derive from {1}", entityType, typeof(TEntity));
            return false;
        }
        if (typeof(IJsonField).IsAssignableFrom(entityType))
            return RegisterFieldInternal(entityType);

        Logger.Error("{0} does not implement IJsonField", entityType);
        return false;
    }

    public bool RegisterEntityTypes(Assembly assembly)
    {
        var retVal = true;

        foreach (var type in assembly.GetTypes()
                                     .Where(x => x.IsAssignableTo(typeof(TEntity))
                                              && typeof(IJsonField).IsAssignableFrom(x)))
        {
            retVal &= RegisterFieldInternal(type);
        }

        return retVal;
    }

    private bool RegisterFieldInternal(Type entityType)
    {
        var attr = entityType.GetCustomAttribute<JsonFieldNameAttribute>(false);
        if (attr == null)
        {
            Logger.Error("{0} is not decorated with a JsonFieldNameAttribute", entityType);
            return false;
        }

        if (entityType.GetConstructor(Type.EmptyTypes) == null)
        {
            Logger.Error("{0} does not have a public parameterless constructor", entityType);
            return false;
        }

        var entityInfo = new EntityInfo(entityType, GenericDeserializer!.MakeGenericMethod(entityType));

        if (EntityTypes.ContainsKey(attr.EntityName))
        {
            EntityTypes[attr.EntityName] = entityInfo;
            Logger.Information<string>("Replaced IJsonField for field '{0}'", attr.EntityName);
        }
        else EntityTypes.Add(attr.EntityName, entityInfo);

        return true;
    }

    protected IEnumerable<FieldDefinition> EnumerateFieldsObject(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateObject())
        {
            var fieldText = fieldObj.Value.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(fieldText, options);
            if (objType == null)
            {
                Logger.Error("Could not determine object type for field object");
                continue;
            }

            yield return new FieldDefinition(objType.Type.ToLower(), fieldText);
        }
    }

    protected IEnumerable<FieldDefinition> EnumerateFieldsArray(JsonElement container, JsonSerializerOptions options)
    {
        foreach (var fieldObj in container.EnumerateArray())
        {
            var fieldText = fieldObj.GetRawText();

            var objType = JsonSerializer.Deserialize<WpType>(fieldText, options);
            if (objType == null)
            {
                Logger.Error("Could not determine object type for field object");
                continue;
            }

            yield return new FieldDefinition(objType.Type.ToLower(), fieldText);
        }
    }

    protected IEnumerable<FieldDefinition> UnsupportedEnumerator(JsonValueKind valueKind)
    {
        Logger.Error("Unsupported fields ValueKind '{0}'", valueKind);
        yield break;
    }
}
