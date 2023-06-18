using System.Diagnostics;
using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOCore;

[JsonObject]
public class RunOperation : BaseOperation
{
    [JsonProperty]
    public string TargetRunDirectory { get; init; } = string.Empty;

    [JsonProperty]
    public string ExeToStart { get; init; } = string.Empty;

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing RunOperation...");
        Logger.Info("Target: {Target}", ExeToStart);
        Logger.Info("RunDirectory: {RunDirectory}", TargetRunDirectory);
        try
        {
            Do(this, context);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing RunOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing RunOperation: {e.Message}");
        }
    }


    public static void Do(RunOperation op, LocationContext context)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(context.BaseTargetContext, op.TargetRunDirectory, op.ExeToStart),
            WorkingDirectory = Path.Combine(context.BaseTargetContext, op.TargetRunDirectory)
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();
    }


    public void Deconstruct(out string targetRunDirectory, out string exeToStart)
    {
        targetRunDirectory = TargetRunDirectory;
        exeToStart = ExeToStart;
    }
}