using Newtonsoft.Json;
using WDBase;
using WDBase.Models;
using WDUtility;

namespace WDOWin.WinGet;

public class WinGetInstallOperation : BaseOperation
{
    [JsonProperty]
    public string ApplicationId { get; init; }

    [JsonProperty]
    public bool RunAsync { get; init; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing WinGetInstallOperation...");
        Logger.Info("Application ID: {AplicationId}", ApplicationId);
        try
        {
            if (RunAsync)
            {
                Logger.Info("Running operation asynchronously...");
                InstallAsync(this).ConfigureAwait(false);
            }
            else
            {
                Install(this);
            }

            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing WinGetInstallOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing WinGetInstallOperation: {e.Message}");
        }
    }


    public static void Install(WinGetInstallOperation op)
    {
        var exitCodeTask = ProcessUtility.RunProcessAsync("WinGet", $"install {op.ApplicationId}");
        exitCodeTask.Wait();
        Logger.Info("Winget install exit code: {0}", exitCodeTask.Result);
    }

    public static async Task InstallAsync(WinGetInstallOperation op)
    {
        var exitCode = await ProcessUtility.RunProcessAsync("WinGet", $"install {op.ApplicationId}")
            .ConfigureAwait(false);
        Logger.Info("Winget install exit code (async): {0}", exitCode);
    }

    public void Deconstruct(out string ApplicationId, out bool RunAsync)
    {
        ApplicationId = this.ApplicationId;
        RunAsync = this.RunAsync;
    }
}