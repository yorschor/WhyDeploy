using NLog;
using WDBase;
using WDBase.Models;
using WDCore.Operations;
using WDCore.Serialisation;
using static WDBase.Models.DeployJob;

namespace WDCore.Generators;

public static class DeployJobGenerator
{
    public static DeployJob New(string name, string shortName)
    {
        return new DeployJob
        {
            Name = name,
            ShortName = shortName,
            JobId = Guid.NewGuid().ToString(),
            ResourceLocation = "",
            CustomBaseTargetLocation = "",
            DeployOperations = new BaseOperation[]
            {
                new CloneOperation { To = "" }
            },
            PurgeOperations = new BaseOperation[]
            {
                new PurgeSelfOperation()
            }
        };
    }

    /// <summary>
    ///     Create a DeployJob on Disk and returns it within a Result if succeeds.
    ///     NOTE: This will attempt to manipulate file in your system! Use with care!
    ///     Default job hierarchy
    ///     ├── JobName
    ///     │   ├── DeployJob.json
    ///     │   └── resources
    /// </summary>
    /// <param name="newJob"></param>
    /// <param name="targetDir"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static Result Generate(this DeployJob newJob, string targetDir, ILogger logger)
    {
        targetDir = Path.Combine(targetDir, newJob.ShortName);
        if (Directory.Exists(targetDir)) return new ErrorResult<DeployJob>($"Directory {targetDir} does already exist");

        try
        {
            Directory.CreateDirectory(targetDir);
            Directory.CreateDirectory(Path.Combine(targetDir, JobDefaultSourceName));
            var serializationResult = WDSerializer.Serialize(newJob);
            switch (serializationResult)
            {
                case SuccessResult<string> successResult:
                    File.WriteAllText(Path.Combine(targetDir, JobDefaultConfigName(newJob.ShortName)),
                        successResult.Data);
                    break;
                case IErrorResult errorResult:
                    logger.Error($"Serialization error: {errorResult.Message}");
                    break;
            }

            // Handle the error as needed, e.g., throw an exception or log the error.
            return new SuccessResult<DeployJob>(newJob);
        }
        catch (Exception ex)
        {
            return new ErrorResult<DeployJob>($"Error creating new job: {ex.Message}");
        }
    }
}