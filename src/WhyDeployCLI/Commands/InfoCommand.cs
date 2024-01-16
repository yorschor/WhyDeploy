using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using WDBase;
using WDCore;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace WhyDeployCLI.Commands;

public class InfoCommand : Command<InfoCommand.Settings>
{
    private const string TitleText = "WhyDeploy";

    private const string DescriptionText = """
                                           WhyDeploy is a simple deployment library meant for small repeatable local tasks.
                                           The core Lib exposes an API for executing "Operations" in order 
                                           to eg. delete/copy files or folders, replacing string within files, Executing Git Commands etc
                                           """;

    private const string CopyRightNoticeText = "\u00a9Ekkehard C. Damisch 2023";

    public override int Execute(CommandContext context, Settings settings)
    {
        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Left"),
                new Layout("Right")
                    .SplitRows(
                        new Layout("Top"),
                        new Layout("Middle"),
                        new Layout("Bottom")));

        var rootPanel = new Panel(layout)
        {
            Border = BoxBorder.None
        };

        var headerFigletPanel = new Panel(new FigletText(TitleText))
        {
            Border = BoxBorder.None
        };
        var descriptionPanel =
            new Panel(new Padder(new Panel(DescriptionText) { Border = BoxBorder.None }, new Padding(5, 5)))
            {
                Border = BoxBorder.Rounded,
                Expand = true
            };
        var copyrightNoticePanel = new Panel(CopyRightNoticeText)
        {
            Border = BoxBorder.Rounded
        };
        var currentConfigPanel = new Panel(GetCurrentConfigTree())
            .Border(BoxBorder.Rounded)
            .Expand().Header("Current Config");
        
        layout["Right"].Ratio = 2;
        layout["Left"].Update(Align.Center(currentConfigPanel, VerticalAlignment.Middle));
        layout["Top"].Update(Align.Center(headerFigletPanel, VerticalAlignment.Middle));
        layout["Middle"].Update(descriptionPanel);
        layout["Bottom"].Update(copyrightNoticePanel);


        AnsiConsole.Write(rootPanel);
        return 0;
    }

    private Tree GetCurrentConfigTree()
    {
        var root = new Tree("");
        root.AddNode("[bold]AppConfig Path:[/]").AddNode(BaseCommandSettings.AppStoragePath);
        root.AddNode("[bold]Current app:[/]").AddNode(BaseCommandSettings.CurrentAppName);
        // Add a node for BaseOperation derived classes
        var baseOperationNode = root.AddNode("[bold]Available Modules:[/]");
        
        // Filter assemblies that start with "WDO"
        ModuleLoader.LoadModules("modules");
        var wdoAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("WDO"));

        foreach (var assembly in wdoAssemblies)
        {
            baseOperationNode.AddNode(assembly.GetName().Name);
        }
        return root;
    }

    public sealed class Settings : BaseCommandSettings
    {
    }
}