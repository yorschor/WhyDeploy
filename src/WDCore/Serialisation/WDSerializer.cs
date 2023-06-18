using WDBase;

namespace WDCore.Serialisation;

public interface IWDSerializer
{
    public Result<string> Serialize<T>(T obj);

    public Result<T> Deserialize<T>(string input);
}

public static class WDSerializer
{
    private static IWDSerializer JsonSerializer { get; } = new WDJsonSerializer();
    private static IWDSerializer YamlSerializer { get; } = new WDYamlSerializer();

    public static Result<string> Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static Result<T> Deserialize<T>(string input)
    {
        return input[..3] == "---" ? YamlSerializer.Deserialize<T>(input) : JsonSerializer.Deserialize<T>(input);
    }
}