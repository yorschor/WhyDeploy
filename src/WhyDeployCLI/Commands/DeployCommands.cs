using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using WDBase;
using WDCore;
using WDUtility;
using WhyDeployCLI.CLIExtension;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global


namespace WhyDeployCLI.Commands;

public class DeploymentSettings : BaseCommandSettings
{
    [CommandArgument(0, "[JOB NAME]")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-l|--local")]
    public bool Local { get; set; } = false;

    public int ExecuteOperations(DeploymentSettings settings, bool deploying = true)
    {
        var w1 = deploying ? "Deploying" : "Purging";
        var w2 = deploying ? "Deployment" : "Purging";
        var successCounter = 0;
        var counter = 0;
        
        var appConfigPath = settings.CurrentAppReference?.PathToApp;
        if (string.IsNullOrEmpty(appConfigPath)) return 1;

        var deployer = new Deployer(appConfigPath);

        if (string.IsNullOrEmpty(settings.Name))
            settings.Name = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose Deployment Job:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(deployer.WDJobs.Select(app => app.Name).ToArray()));

        var root = new Tree($"{w1} job '{settings.Name}'... ");
        AnsiConsole.Live(root).Start(ctx =>
        {
            deployer.OperationCompleted += (_, e) =>
            {
                counter++;
                var panel = new Panel($"{e.Operation}")
                {
                    Border = BoxBorder.Rounded
                };
                root.AddNode(panel);

                if (e.ExecutionResult.Success)
                {
                    panel.Header = new PanelHeader(" :check_mark: [green]SUCCESS[/] ");
                    successCounter++;
                }
                else if (e.ExecutionResult is IErrorResult r)
                {
                    panel.Header = new PanelHeader(" :cross_mark: [red]FAILURE[/] ");
                    root.AddNode($"[red]     Error Message: {r.Message}[/] \n \n");
                }
                ctx.Refresh();
            };

            var result = deploying ? deployer.Deploy(settings.Name) : deployer.Purge(settings.Name);

            if (!result.HandleIfError())
            {
                root.AddNode($"[red]{w2} failed![/]");
                return 1;
            }

            root.AddNode(successCounter < counter
                ? $"[red]{w2} partially failed with[/] [green]{successCounter} successful[/] [red]and {counter - successCounter} Errors![/]"
                : $"[green]{w2} completed successfully! {successCounter}/{successCounter} [/]");
            return 0;
        });
        return 0;
    }
}

public class DeployCommand : Command<DeployCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        return settings.ExecuteOperations(settings);
    }

    public sealed class Settings : DeploymentSettings
    {
    }
}

public class PurgeCommand : Command<PurgeCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        return settings.ExecuteOperations(settings, false);
    }

    public sealed class Settings : DeploymentSettings
    {
    }
}

public class ListCommand : Command<ListCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        var appSettingsPath = settings.CurrentAppReference?.PathToApp;

        if (string.IsNullOrEmpty(appSettingsPath) || !LogHelper.FileExists(appSettingsPath, settings.Logger))
        {
            AnsiConsole.Write(PrettyLogHelper.GetNoAppConfigFoundResponse(appSettingsPath ?? "Empty"));
            return 1;
        }

        var deployer = new Deployer(appSettingsPath);
        deployer.SetLogger(settings.Logger);

        if (string.IsNullOrEmpty(settings.Name))
        {
            var root = new Tree($"Listing all Jobs");
            root.AddNode("Path").AddNode(appSettingsPath);

            //var subTree = new Tree("");
            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            
            deployer.FindAllJobs();
            foreach (var job in deployer.WDJobs)
                // subTree.AddNode($"JobName: {job.Name}, ShortName: {job.ShortName}, JobID: {job.JobId}");
                grid.AddRow(
                    new Markup("*"),
                    new Markup($"[bold]JobName:[/] {job.Name}"),
                    new Markup($"[bold]ShortName:[/] {job.ShortName}"),
                    new Markup($"[bold]JobID:[/] {job.JobId}"));
            
            root.AddNode(new Panel(grid)
                .Border(BoxBorder.Rounded)
                .Header(BaseCommandSettings.CurrentAppName)
                .Collapse());
            root.AddNode($"End. Total job count {grid.Rows.Count}");
            AnsiConsole.Write(root);
        }
        else
        {
            var jobResult = deployer.WDJobs.Get(settings.Name);
            if (!jobResult.HandleIfError()) return 1;
            var root = new Tree($"{BaseCommandSettings.CurrentAppName} --> Inspecting job {jobResult.Data.Name}");
            var deploySubNode = root.AddNode("Deploy Operations");
            TreeHelper.InsertOperationsTreeArray(deploySubNode,
                jobResult.Data.DeployOperations ?? Array.Empty<BaseOperation>());
            var purgeSubNode = root.AddNode("Purge Operations");
            TreeHelper.InsertOperationsTreeArray(purgeSubNode,
                jobResult.Data.PurgeOperations ?? Array.Empty<BaseOperation>());

            AnsiConsole.Write(root);
        }

        return 0;
    }


    public sealed class Settings : BaseCommandSettings
    {
        [CommandArgument(0, "[JOB NAME]")]
        [Description("Name of the job. If not specified, lists all jobs for current app")]
        public string Name { get; set; } = string.Empty;
    }
}