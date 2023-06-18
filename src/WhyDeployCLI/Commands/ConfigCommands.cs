using Spectre.Console;
using Spectre.Console.Cli;
using WDCore.StorageHelper;
using WhyDeployCLI.CLIExtension;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#pragma warning disable CS8618
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global


namespace WhyDeployCLI.Commands;

#region BaseSettings

public class ConfigSettings : BaseCommandSettings
{
}

#endregion

#region Commands

public class ListAppsCommand : Command<ListAppsCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var deployAppsResult = WDAppStorageHelper.GetAppStorage(BaseCommandSettings.AppStoragePath);
            if (!deployAppsResult.HandleIfError()) return 1;
            var deployApps = deployAppsResult.Data;

            var prettyPath = new TextPath(BaseCommandSettings.AppStoragePath);

            var root = new Tree("Listing registered applications");
            root.AddNode("Path: ").AddNode(prettyPath);
            var appsTree = new Tree("");
            foreach (var deployApp in deployApps!)
            {
                var subNode = appsTree.AddNode($"[springgreen4]{deployApp.Name}[/]");
                subNode.AddNode($"Config Path -> {deployApp.PathToApp}");
            }

            root.AddNode(new Panel(appsTree)
                .Border(BoxBorder.Rounded)
                .Header("Apps")
                .Collapse());

            root.AddNode($"Total app count [blue]{appsTree.Nodes.Count}[/]");
            AnsiConsole.Write(root);
        }
        catch (Exception e)
        {
            settings.Logger.Error(e);
            return 1;
        }

        return 0;
    }

    public sealed class Settings : ConfigSettings
    {
    }
}

public class AddAppToStorageCommand : Command<AddAppToStorageCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var p = Path.GetFullPath(settings.Path);
            var a = WDAppStorageHelper.GenerateAppRefFromConfigPath(p);
            if (!a.HandleIfError()) return 1;
            var result =
                WDAppStorageHelper.AddAppToStorage(a.Data with { PathToApp = p }, BaseCommandSettings.AppStoragePath);
            if (!result.HandleIfError()) return 1;
            AnsiConsole.WriteLine($"Successfully added {BaseCommandSettings.CurrentAppName} to app storage!");
        }
        catch (Exception e)
        {
            settings.Logger.Error(e.Message);
            return 1;
        }

        return 0;
    }

    public sealed class Settings : ConfigSettings
    {
        [CommandArgument(0, "<PATH>")]
        public string Path { get; set; }
    }
}

public class SetPathToAppStorageCommand : Command<SetPathToAppStorageCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        var p = Path.GetFullPath(settings.Path);
        if (!File.Exists(settings.Path)) PrettyLogHelper.PrintError("Path cannot be set. No appStorage found at " + p);
        BaseCommandSettings.AppStoragePath = p;
        return 0;
    }

    public sealed class Settings : ConfigSettings
    {
        [CommandArgument(0, "<PATH>")]
        public string Path { get; set; }
    }
}

public class SelectAppCommand : Command<SelectAppCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            string message;
            var deployAppsResult = WDAppStorageHelper.GetAppStorage(BaseCommandSettings.AppStoragePath);
            if (!deployAppsResult.HandleIfError()) return 1;
            var deployApps = deployAppsResult.Data;

            var newSelectedApp = "";
            foreach (var deployApp in deployApps!.Where(deployApp => deployApp.Name == settings.Name))
                newSelectedApp = deployApp.Name;

            if (string.IsNullOrEmpty(newSelectedApp))
                newSelectedApp = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose Deployment App:")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                        .AddChoices(deployApps.Select(app => app.Name).ToArray()));

            if (string.IsNullOrEmpty(newSelectedApp))
            {
                message = $"No App with Name: ''{settings.Name}'' found! ";
                settings.Logger.Warn(message);
                PrettyLogHelper.PrintWarning(message);
                return 1;
            }

            BaseCommandSettings.CurrentAppName = newSelectedApp;
            message = $"Current app set to {newSelectedApp}";
            settings.Logger.Info(message);
            AnsiConsole.WriteLine("-> " + message);
            return 0;
        }
        catch (Exception e)
        {
            settings.Logger.Error(e);
            return 1;
        }
    }

    public sealed class Settings : ConfigSettings
    {
        [CommandArgument(0, "[APP NAME]")]
        public string Name { get; set; }
    }
}

public class RevalidateAppStorageCommand : Command<RevalidateAppStorageCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("[gray]Validating AppStorage[/]");
        var result = WDAppStorageHelper.RevalidateStorage(BaseCommandSettings.AppStoragePath);
        if (!result.HandleIfError()) return 1;
        AnsiConsole.MarkupLine("[green]Successfully validate AppStorage[/]");
        return 0;
    }

    public sealed class Settings : ConfigSettings
    {
    }
}

#endregion