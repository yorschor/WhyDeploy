using NLog;
using Spectre.Console.Cli;
using WDBase.Models;
using WDCore.StorageHelper;
using WhyDeployCLI.CLIExtension;
using WhyDeployCLI.Commands;
using WhyDeployCLI.Properties;

#pragma warning disable CS8618

namespace WhyDeployCLI;

public static class WhyDeployCli
{
    public static int Main(string[] args)
    {
        var app = new CommandApp();
        
        app.Configure(config =>
        {
            config.SetApplicationName("whyDeploy");

            // Config
            config.AddBranch<ConfigSettings>("config", configBranch =>
            {
                configBranch.SetDescription("Configuration commands");
                
                configBranch.AddCommand<ListAppsCommand>("list")
                    .WithDescription("lists all apps that are currently registered")
                    .WithExample("config", "list");

                configBranch.AddCommand<AddAppToStorageCommand>("add")
                    .WithDescription("Adds a existing app to storage")
                    .WithExample("config", "add", "/path/To/App");

                configBranch.AddCommand<SetPathToAppStorageCommand>("setPath")
                    .WithDescription("Sets the path to an existing app storage replacing the old one")
                    .WithExample("config", "setPath", "/path/To/Storage");
                configBranch.AddCommand<RevalidateAppStorageCommand>("revalidate")
                    .WithDescription("Revalidates the current appstore deleting entries that no longer exists on disk")
                    .WithExample("config", "revalidate");
            });
            
            // New
            config.AddBranch<NewStuffSettings>("new", newBranch =>
            {
                newBranch.SetDescription("Creation commands");

                newBranch.AddCommand<NewAppCommand>("app")
                    .WithDescription("Create a new app with the specified name and adds it to the appStorage");
                newBranch.AddCommand<NewJobCommand>("job")
                    .WithDescription("Create a new Job with the specified name");
                newBranch.AddCommand<NewAppStorageCommand>("appStorage")
                    .WithDescription("Create a new Job with the specified name")
                    .WithExample("config", "create", "../Path/To/New/AppStorage");
            });

            config.AddCommand<SelectAppCommand>("select")
                .WithDescription("Selects a known app as the current one")
                .WithExample("selectApp", "[App Name]");

            config.AddCommand<DeployCommand>("deploy")
                .WithDescription("Deploy a specific Job")
                .WithExample("deploy", "MyDeployJob");

            config.AddCommand<PurgeCommand>("purge")
                .WithDescription("Purge a specific Job")
                .WithExample("purge", "MyDeployJob");

            config.AddCommand<ListCommand>("list")
                .WithDescription("List all operations in a given job")
                .WithExample("list");

            config.AddCommand<InfoCommand>("info")
                .WithDescription("Displays information about WhyDeploy")
                .WithExample("info");

            // config.AddCommand<FortyCommand>("40")
            //     .WithDescription("Attempts to setup your console for use with WhyDeploy")
            //     .WithExample("40");
            config.AddCommand<InitCommand>("init")
                .WithDescription("Initialize WhyDeploy app storage")
                .WithExample("init");

            

            // Utility
            config.AddBranch<UtilitySettings>("util", util =>
            {
                util.SetDescription("Miscellaneous utilities");
                util.AddCommand<ConcatCommand>("concat")
                    .WithDescription("Concatenate Files within a folder")
                    .WithExample("concat", "*.cs", "/source", "output.txt   ");
            });
        });

        return app.Run(args);
    }
}

public class BaseCommandSettings : CommandSettings
{
    public readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public static string AppStoragePath
    {
        get => Settings.Default.AppStoragePath;
        set
        {
            Settings.Default.AppStoragePath = value;
            Settings.Default.Save();
        }
    }

    public static string CurrentAppName
    {
        get => Settings.Default.CurrentApp;
        set
        {
            Settings.Default.CurrentApp = value;
            Settings.Default.Save();
        }
    }

    public DeployApp? CurrentApp
    {
        get
        {
            var ret = WDAppStorageHelper.GetAppConfigByName(AppStoragePath, CurrentAppName, Logger);
            return ret.HandleIfError() ? ret.Data : null;
        }
    }

    public DeployAppReference? CurrentAppReference
    {
        get
        {
            var ret = WDAppStorageHelper.GetAppRefByName(AppStoragePath, CurrentAppName, Logger);
            return ret.HandleIfError() ? ret.Data : null;
        }
    }
}