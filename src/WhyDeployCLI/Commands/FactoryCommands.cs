using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using WDBase;
using WDBase.Models;
using WDCore.Generators;
using WDCore.StorageHelper;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace WhyDeployCLI.Commands;

#region BaseSettings

public class NewStuffSettings : BaseCommandSettings
{
}

#endregion

#region Commands

public class NewJobCommand : Command<NewJobCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        var p = settings.Path;
        if (string.IsNullOrEmpty(p))
            //Check current app if no app is set select current directory
            p = settings.CurrentApp is not null
                ? settings.CurrentApp.Value.JobsDirectory
                : Directory.GetCurrentDirectory();

        p = Path.GetFullPath(p);

        var root = new Tree("[bold blue] Job creation with default settings[/]");
        root.AddNodes($"[bold]Job Name:[/] [italic gray]{settings.Name}[/]", $"[bold]Path :[/] {p}");
        var newAppInfoPanel = new Panel(root).Header("WhyDeploy AppFactory");

        AnsiConsole.Live(newAppInfoPanel)
            .Start(ctx =>
            {
                settings.Logger.Info($"Creating job '{settings.Name}' at {p} with default settings...");
                var newJob = DeployJobGenerator.New(settings.Name, settings.Name);
                var r = newJob.Generate(p, settings.Logger);
                if (r is IErrorResult err)
                {
                    newAppInfoPanel.BorderColor(Color.Red);
                    root.AddNode($"[red]Error during JobCreation:[/] {err.Message}");
                    ctx.Refresh();
                    return 1;
                }

                newAppInfoPanel.BorderColor(Color.Green);
                root.AddNode("[green]Done![/]");
                ctx.Refresh();
                return 0;
            });


        return 0;
    }

    public sealed class Settings : NewStuffSettings
    {
        [CommandArgument(0, "<JOB NAME>")]
        [Description("Name of the created job")]
        public string Name { get; set; } = "NewJob";

        [CommandArgument(1, "[PATH]")]
        [Description(
            "Path to the created job. Defaults to job directory of currently selected app or current folder if no app is selected")]
        public string Path { get; set; } = "";
    }
}

public class NewAppCommand : Command<NewAppCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        var p = settings.Path;
        if (string.IsNullOrEmpty(p))
        {
            p = BaseCommandSettings.AppStoragePath.Replace("WhyDeployAppStorage.json", "");
            if (!Path.Exists(p)) p = Directory.GetCurrentDirectory();
        }

        p = Path.GetFullPath(p);

        settings.Logger.Info($"Creating app '{settings.Name}' at {p} with default settings...");

        var root = new Tree("[bold blue] App creation with default settings[/]");
        root.AddNodes($"[bold]App Name:[/] [italic gray]{settings.Name}[/]", $"[bold]AppStoragePath:[/] {p}");
        var newAppInfoPanel = new Panel(root).Header("WhyDeploy AppFactory");
        AnsiConsole.Live(newAppInfoPanel)
            .Start(ctx =>
            {
                var newAppConfig = DeployAppGenerator.New(settings.Name, p);
                var r = newAppConfig.Generate(p, settings.Logger);
                if (r is IErrorResult err)
                {
                    newAppInfoPanel.BorderColor(Color.Red);
                    root.AddNode($"[red]Error during AppCreation:[/] {err.Message}");
                    ctx.Refresh();
                    return 1;
                }

                var appConfigPath = Path.Combine(p, settings.Name, DeployApp.DefaultAppConfigName(newAppConfig.Name));
                var newAppReference =
                    new DeployAppReference(newAppConfig.Name, appConfigPath, newAppConfig.AppId);
                WDAppStorageHelper.AddAppToStorage(newAppReference, BaseCommandSettings.AppStoragePath);

                newAppInfoPanel.BorderColor(Color.Green);
                root.AddNode("[green]Done![/]");
                root.AddNode("Generated Structure").AddNode(newAppConfig.Name)
                    .AddNodes(DeployApp.DefaultJobDirectory, DeployApp.DefaultSharedJobResourceLocation,
                        DeployApp.DefaultAppConfigName(newAppConfig.Name));
                ctx.Refresh();
                return 0;
            });
        var selectAsCurrent = AnsiConsole.Confirm($"Select new app {settings.Name} as current one?");
        if (selectAsCurrent)
        {
            BaseCommandSettings.CurrentAppName = settings.Name;
            AnsiConsole.Markup("[green]Done[/]");
        }

        return 0;
    }

    public sealed class Settings : NewStuffSettings
    {
        [CommandArgument(0, "<APP NAME>")]
        [Description("Name of the created app")]
        public string Name { get; set; } = "NewApp";

        [CommandArgument(1, "[PATH]")]
        [Description("Path to the created app. Defaults to AppStoragePath")]
        public string Path { get; set; } = "";
    }
}

public class NewAppStorageCommand : Command<NewAppStorageCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        return SharedCommandMethods.CreateAppStorage(settings.Path);
    }

    public sealed class Settings : NewStuffSettings
    {
        [CommandArgument(0, "<PATH>")]
        public string Path { get; set; }
    }
}

#endregion