using Newtonsoft.Json;
using WDBase;

namespace WDCore.Serialisation;

public class WDJsonSerializer : IWDSerializer
{
    private static JsonSerializerSettings GetSerializerSettings =>
        new()
        {
            TypeNameHandling = TypeNameHandling.Objects,
            SerializationBinder = new ShortTypeNameBinder(),
            Formatting = Formatting.Indented
        };

    public Result<string> Serialize<T>(T obj)
    {
        try
        {
            var serializedData = JsonConvert.SerializeObject(obj, GetSerializerSettings);
            return new SuccessResult<string>(serializedData);
        }
        catch (Exception e)
        {
            return new ErrorResult<string>("Failed to serialize object to JSON.",
                new List<Error> { new("SerializationError", e.Message) });
        }
    }

    public Result<T> Deserialize<T>(string json)
    {
        try
        {
            var deserializedObject = JsonConvert.DeserializeObject<T>(json, GetSerializerSettings)!;
            return new SuccessResult<T>(deserializedObject);
        }
        catch (Exception e)
        {
            return new ErrorResult<T>("Failed to deserialize JSON to object.",
                new List<Error> { new("DeserializationError", e.Message) });
        }
    }
}