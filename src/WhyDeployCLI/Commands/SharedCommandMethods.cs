using Newtonsoft.Json;
using Spectre.Console;
using WDBase.Models;
using WDUtility;
using WhyDeployCLI.CLIExtension;

namespace WhyDeployCLI.Commands;

public class SharedCommandMethods
{
    public static int CreateAppStorage(string path)
    {
        var newPath = path ?? BaseCommandSettings.AppStoragePath.Replace("WhyDeployAppStorage.json", "");
        newPath = Path.GetFullPath(newPath);
        newPath = Path.Combine(newPath, "WhyDeployAppStorage.json");
        if (File.Exists(newPath))
        {
            PrettyLogHelper.PrintInfo("AppStorage already exists at " + newPath);
        }
        else
        {
            var proceed = AnsiConsole.Confirm($"Creating new AppStorage file at {newPath}. Proceed?");
            if (!proceed)
            {
                AnsiConsole.WriteLine("Aborting AppStorage creation");
                return 1;
            }

            var newAppConfig = JsonConvert.SerializeObject(new List<DeployAppReference>());
            FileSystemHelper.WriteToFileWithPathInsurance(newPath, newAppConfig);
            AnsiConsole.WriteLine("Created new AppStorage at " + newPath);
            BaseCommandSettings.AppStoragePath = newPath;
        }

        return 0;
    }
}