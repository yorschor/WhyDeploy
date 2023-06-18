using Spectre.Console;
using Spectre.Console.Cli;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace WhyDeployCLI.Commands;

public class FortyCommand : Command<FortyCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write("Not implemented et :( ");
        return 0;
    }


    public sealed class Settings : BaseCommandSettings
    {
    }
}

public class InitCommand : Command<InitCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("Initializing WhyDeploy");
        if (!string.IsNullOrEmpty(BaseCommandSettings.AppStoragePath))
        {
            var proceed = AnsiConsole.Confirm($"WhyDeploy seems to be already present on the system. AppStorage at {BaseCommandSettings.AppStoragePath}. Proceed?");
            if (!proceed)
            {
                AnsiConsole.MarkupLine("Aborting initialisation");
                return 1;
            }
        }

        var userPath = @"%USERPROFILE%/AppData/Local/WhyDeploy/";
        userPath = Environment.ExpandEnvironmentVariables(userPath);
        userPath = AnsiConsole.Ask("Choose directory for WhyDeploy AppStorage file: default =  ", userPath);
        var intResult = SharedCommandMethods.CreateAppStorage(userPath);
        if (intResult == 1) return 1;
        AnsiConsole.MarkupLine("[green]Initialization complete![/]");
        return 0;
    }

    public sealed class Settings : BaseCommandSettings
    {
    }
}