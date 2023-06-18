namespace WDBase.Models;

public record struct DeployApp(
    string Name,
    string SharedJobResourceLocation,
    string AppId,
    string JobsDirectory,
    string JobBaseTargetLocation
)
{
    public static string DefaultAppConfigName(string name) => $"deployApp.{name}.json";
    public const string DefaultTargetLocation = "deployed";
    public const string DefaultJobDirectory = "JobDirectory";
    public const string DefaultSharedJobResourceLocation = "SharedJobResources";
}