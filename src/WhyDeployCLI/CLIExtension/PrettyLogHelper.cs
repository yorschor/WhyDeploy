using Spectre.Console;
using Spectre.Console.Rendering;
using WDBase;

namespace WhyDeployCLI.CLIExtension;

public static class PrettyLogHelper
{
    private static readonly object LockObject = new();

    /// <summary>
    ///     Handles the error result by printing the error message if the result is of type IErrorResult.
    /// </summary>
    /// <param name="r">The result object to be checked and handled.</param>
    /// <returns>Returns true if the result is not of type IErrorResult, otherwise prints the error message and returns false.</returns>
    public static bool HandleIfError(this Result r)
    {
        if (r is not IErrorResult e) return true;
        PrintError(e.Message);
        return false;
    }

    public static void PrintError(string message)
    {
        lock (LockObject)
        {
            var tree = new Tree("[red]WD Error[/]");
            tree.AddNode(message);
            AnsiConsole.Write(tree);
        }
    }

    public static void PrintInfo(string message)
    {
        lock (LockObject)
        {
            var tree = new Tree("[cyan]WD Info[/]");
            tree.AddNode(message);
            AnsiConsole.Write(tree);
        }
    }

    public static void PrintWarning(string message)
    {
        lock (LockObject)
        {
            var tree = new Tree("[yellow]WD Warning[/]");
            tree.AddNode(message);
            AnsiConsole.Write(tree);
        }
    }

    public static IRenderable GetNoAppStorageFoundResponse()
    {
        return new Panel("No app storage path is configured or app storage file is corrupt").Header("Error")
            .BorderColor(Color.Red);
    }

    public static IRenderable GetNoAppConfigFoundResponse(string appPath)
    {
        return new Panel($"No app config found at path ''{appPath}''").Header("Error")
            .BorderColor(Color.Red);
    }
}