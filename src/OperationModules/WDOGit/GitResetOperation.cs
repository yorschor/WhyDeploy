using Newtonsoft.Json;
using WDBase;
using WDBase.Models;
using WDUtility;

namespace WDOGit;

[JsonObject]
public class GitResetOperation : BaseOperation
{
    [JsonProperty]
    public string RepositoryPath { get; init; } = string.Empty;

    [JsonProperty]
    public bool Stash { get; init; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing GitResetOperation...");
        Logger.Info("Repository Path: {0}", RepositoryPath);
        try
        {
            if (Stash)
            {
                Logger.Info("Stashing changes...");
                StashChanges();
            }

            ResetRepository();
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing GitResetOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing GitResetOperation: {e.Message}");
        }
    }


    private void StashChanges()
    {
        var exitCodeTask = ProcessUtility.RunProcessAsync("git", $"-C {RepositoryPath} stash");
        exitCodeTask.Wait();
        Logger.Info("Git stash exit code: {ExitCode}", exitCodeTask.Result);
    }

    private void ResetRepository()
    {
        var fetchExitCodeTask = ProcessUtility.RunProcessAsync("git", $"-C {RepositoryPath} fetch origin");
        fetchExitCodeTask.Wait();
        Logger.Info("Git fetch exit code: {ExitCode}", fetchExitCodeTask.Result);

        var resetExitCodeTask = ProcessUtility.RunProcessAsync("git", $"-C {RepositoryPath} reset --hard");
        resetExitCodeTask.Wait();
        Logger.Info("Git reset exit code: {ExitCode}", resetExitCodeTask.Result);

        var cleanExitCodeTask = ProcessUtility.RunProcessAsync("git", $"-C {RepositoryPath} clean -fdx");
        cleanExitCodeTask.Wait();
        Logger.Info("Git clean exit code: {ExitCode}", cleanExitCodeTask.Result);
    }

    public void Deconstruct(out string RepositoryPath, out bool Stash)
    {
        RepositoryPath = this.RepositoryPath;
        Stash = this.Stash;
    }
}