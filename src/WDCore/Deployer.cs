using NLog;
using WDBase;
using WDBase.Collections;
using WDBase.Models;
using WDCore.Events;
using WDCore.Serialisation;
using WDUtility;

// ReSharper disable MemberCanBePrivate.Global

namespace WDCore;

public class Deployer
{
    private const string DefaultDeployFileName = "job.*.*";
    private readonly LocationContext _context;

    public readonly DeployApp DeployApp;
    private LocationContext _currentContext;
    public ILogger Logger = LogManager.GetCurrentClassLogger();

    public Deployer(string appConfigPath = "DeployAppConfig.json")
    {
        ModuleLoader.LoadModules("modules");
        var json = File.ReadAllText(appConfigPath);
        WDJobs = new WDJobCollection();

        var deserializationResult = WDSerializer.Deserialize<DeployApp>(json);
        if (deserializationResult is IErrorResult errorResult)
        {
            Logger.Error($"While deserializing app config: {errorResult.Message}");
            foreach (var error in errorResult.Errors) Logger.Error($"{error.Code}: {error.Details}");
        }

        DeployApp = deserializationResult.Data;

        _context = new LocationContext(Path.GetDirectoryName(appConfigPath)!, DeployApp.SharedJobResourceLocation,
            DeployApp.JobBaseTargetLocation);
        _currentContext = _context;
        SetUp();
    }
    
    public WDJobCollection WDJobs { get; }

    public event EventHandler<OperationResultEventArgs>? OperationCompleted;


    private void SetUp()
    {
        FindAllJobs();
    }

    public void SetLogger(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    ///     Finds all jobs in the application's cache folder, deserializes them, and adds them to the _jobs dictionary.
    ///     Optionally prints the job names and short names to the console.
    /// </summary>
    /// <param name="printOutput">True to print job names and short names to the console; false otherwise. Default is false.</param>
    public void FindAllJobs(bool printOutput = false)
    {
        WDJobs.Clear();

        var deployFiles = FileSystemHelper.FindFilesByName(DeployApp.JobsDirectory, DefaultDeployFileName);
        foreach (var jobPath in deployFiles)
        {
            var jobJson = File.ReadAllText(jobPath);
            var deserializationResult = WDSerializer.Deserialize<DeployJob>(jobJson);

            if (deserializationResult.Failure)
            {
                if (deserializationResult is IErrorResult errorResult && printOutput)
                {
                    Logger.Error($"Error deserializing job at {jobPath}: {errorResult.Message}");
                    errorResult.PrintAll();
                }

                continue; // Skip this iteration and move to the next file.
            }

            var job = deserializationResult.Data;
            WDJobs.Add(job);

            if (printOutput)
                Logger.Info($"JobName: {job.Name}, ShortName: {job.ShortName}, JobID: {job.JobId}");
        }
    }

    private void LoadNewJob(DeployJob job)
    {
        var jobSourceDir = string.IsNullOrEmpty(job.ResourceLocation) || job.ResourceLocation == "."
            ? DeployApp.SharedJobResourceLocation
            : Path.Combine(_context.BaseWorkingContext, job.ResourceLocation);
        var jobTargetDir = string.IsNullOrEmpty(job.CustomBaseTargetLocation) || job.CustomBaseTargetLocation == "."
            ? DeployApp.JobBaseTargetLocation
            : Path.Combine(_context.BaseTargetContext, job.CustomBaseTargetLocation);
        var jobWorkingDir = _context.BaseWorkingContext;

        _currentContext = new LocationContext(jobWorkingDir, jobSourceDir, jobTargetDir);
        Logger.Info($"Loaded NewJob with {_currentContext}");
    }

    public Result Deploy(string jobName)
    {
        var result = WDJobs.Get(jobName);
        if (result.Failure) return new ErrorResult($"Failed to find job with name '{jobName}'.");

        var job = result.Data;
        return Deploy(job);
    }


    public Result Deploy(DeployJob job)
    {
        try
        {
            LoadNewJob(job);
            ExecuteOperations(job.DeployOperations ?? Array.Empty<BaseOperation>());
        }
        catch (Exception e)
        {
            return new ErrorResult($"Error in Deploy: {e.Message}",
                new List<Error> { new("DeployError", e.StackTrace ?? string.Empty) });
        }

        return new SuccessResult();
    }


    public Result Purge(string jobName)
    {
        var result = WDJobs.Get(jobName);
        if (result.Failure) return new ErrorResult($"Failed to find job with name '{jobName}'.");

        var job = result.Data;
        // InspectJob(jobName);
        return Purge(job);
    }


    public Result Purge(DeployJob job)
    {
        try
        {
            LoadNewJob(job);
            ExecuteOperations(job.PurgeOperations ?? Array.Empty<BaseOperation>());
        }
        catch (Exception e)
        {
            return new ErrorResult($"Error in Purge: {e.Message}",
                new List<Error> { new("PurgeError", e.StackTrace ?? string.Empty) });
        }

        return new SuccessResult();
    }

    private void ExecuteOperations(IEnumerable<BaseOperation> operations)
    {
        foreach (var op in operations)
        {
            var result = op.Execute(_currentContext);
            OperationCompleted?.Invoke(this, new OperationResultEventArgs(op, result));
        }
    }
}