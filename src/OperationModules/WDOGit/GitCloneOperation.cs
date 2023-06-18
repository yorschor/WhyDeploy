using LibGit2Sharp;
using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOGit;

[JsonObject]
public class CloneGitRepositoryOperation : BaseOperation
{
    [JsonProperty]
    public string RepositoryUrl { get; init; } = string.Empty;

    [JsonProperty]
    public string Destination { get; init; } = string.Empty;

    [JsonProperty]
    public bool RunAsync { get; init; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing CloneGitRepositoryOperation...");
        Logger.Info("Repository URL: {RepoUrl}", RepositoryUrl);
        Logger.Info("Destination: {Destination}", Path.Combine(context.BaseTargetContext, Destination));
        try
        {
            if (RunAsync)
            {
                Logger.Info("Running operation asynchronously...");
                DoAsync(this, context).ConfigureAwait(false);
            }
            else
            {
                Do(this, context);
            }

            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing CloneGitRepositoryOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing CloneGitRepositoryOperation: {e.Message}");
        }
    }


    public static void Do(CloneGitRepositoryOperation op, LocationContext context)
    {
        var destinationPath = Path.Combine(context.BaseTargetContext, op.Destination);
        Repository.Clone(op.RepositoryUrl, destinationPath);
    }

    public static async Task DoAsync(CloneGitRepositoryOperation op, LocationContext context)
    {
        var destinationPath = Path.Combine(context.BaseTargetContext, op.Destination);
        await Task.Run(() => Repository.Clone(op.RepositoryUrl, destinationPath)).ConfigureAwait(false);
    }


    public void Deconstruct(out string repositoryUrl, out string destination, out bool runAsync)
    {
        repositoryUrl = RepositoryUrl;
        destination = Destination;
        runAsync = RunAsync;
    }
}