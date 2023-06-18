using Newtonsoft.Json;
using NLog;
using WDBase;
using WDBase.Models;
using static WDBase.Models.DeployApp;
using static WDUtility.FileSystemHelper;

namespace WDCore.Generators;

public static class DeployAppGenerator
{
    public static DeployApp New(string name, string target)
    {
        var targetDir = string.IsNullOrEmpty(name) ? target : Path.Combine(target, name);
        return new DeployApp
        {
            Name = name,
            AppId = Guid.NewGuid().ToString(),
            JobsDirectory = CombinePaths(targetDir, DefaultJobDirectory),
            SharedJobResourceLocation = CombinePaths(targetDir, DefaultSharedJobResourceLocation),
            JobBaseTargetLocation = CombinePaths(targetDir, DefaultTargetLocation)
        };
    }

    /// <summary>
    ///     Writes a DeployAppConfig to Disk if it doesnt exists.
    ///     NOTE: This will attempt to manipulate file in your system! Use with care!
    ///     Default app hierarchy
    ///     ├── AppName
    ///     │   ├── DeployAppConfig.json
    ///     │   ├── JobDirectory
    ///     │   ├── deployed
    ///     │   └── SharedJobResources
    /// </summary>
    /// <param name="newApp"></param>
    /// <param name="targetDir">The directory in which the new directory with AppName will be generated into</param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static Result Generate(this DeployApp newApp, string targetDir, ILogger logger)
    {
        var basePath = CombinePaths(targetDir, newApp.Name);
        if (Directory.Exists(basePath))
            return new ErrorResult<DeployApp>(
                $"App with name {newApp.Name} already exists at {basePath}!");

        Directory.CreateDirectory(basePath);
        Directory.CreateDirectory(newApp.JobsDirectory);
        Directory.CreateDirectory(newApp.SharedJobResourceLocation);
        Directory.CreateDirectory(newApp.JobBaseTargetLocation);

        File.WriteAllText(CombinePaths(basePath, DefaultAppConfigName(newApp.Name)), JsonConvert.SerializeObject(newApp));
        return new SuccessResult();
    }
}