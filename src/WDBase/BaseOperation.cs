using System.Text;
using Newtonsoft.Json;
using NLog;
using WDBase.Models;

namespace WDBase;

public abstract class BaseOperation
{
    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public string OperationName => GetType().Name;
    public abstract Result Execute(LocationContext context);

    public sealed override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"{OperationName}");

        var properties = GetOperationProperties();
        foreach (var prop in properties) builder.AppendLine($"   * {prop.Key} -> {prop.Value}");

        return builder.ToString();
    }

    public IDictionary<string, object> GetOperationProperties()
    {
        var properties = new Dictionary<string, object>();

        var jsonProps = GetType().GetProperties()
            .Where(prop => prop.Name != nameof(OperationName))
            .Select(prop => new
            {
                PropName = ((JsonPropertyAttribute)Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))!)
                    ?.PropertyName ?? prop.Name,
                PropValue = prop.GetValue(this)
            });

        foreach (var prop in jsonProps) properties[prop.PropName] = prop.PropValue!;

        return properties;
    }
}