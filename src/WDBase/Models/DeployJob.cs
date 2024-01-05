using System.Text;
using Newtonsoft.Json;

namespace WDBase.Models;

public record struct DeployJob(
    [JsonProperty] string Name,
    [JsonProperty] string ShortName,
    [JsonProperty] string JobId,
    [JsonProperty] string ResourceLocation,
    [JsonProperty] string CustomBaseTargetLocation,
    [JsonProperty] BaseOperation[]? DeployOperations,
    [JsonProperty] BaseOperation[]? PurgeOperations
)
{
    public static string JobDefaultConfigName(string jobName) => $"job.{jobName}.json";
    public const string JobDefaultSourceName = "resources";

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Deploy Operations");
        builder.Append(PrintOperations(DeployOperations));
        builder.AppendLine("Purge Operations");
        builder.Append(PrintOperations(PurgeOperations));
        return builder.ToString();
    }

    private static string PrintOperations(IEnumerable<BaseOperation>? operations)
    {
        if (operations is not BaseOperation[] baseOperations) return string.Empty;

        if (baseOperations.Length == 0) return string.Empty;
        var sb = new StringBuilder();
        foreach (var o in baseOperations) sb.Append(o);

        return sb.ToString();
    }
}