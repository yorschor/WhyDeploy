using System.ComponentModel;
using Spectre.Console.Cli;
using WDUtility;

#pragma warning disable CS8618
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace WhyDeployCLI.Commands;

#region BaseSettings

public class UtilitySettings : BaseCommandSettings
{
}

#endregion

public class ConcatCommand : Command<ConcatCommand.Settings>
{
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override int Execute(CommandContext context, Settings settings)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        settings.Logger.Info($"Concatenating files matching the pattern '{settings.FileExtension}'...");
        FileSystemHelper.ConcatenateFiles(settings.Path, settings.FileExtension, settings.Output);
        return 0;
    }

    public sealed class Settings : UtilitySettings
    {
        [CommandArgument(0, "<PATTERN>")]
        [Description("Path pattern for the files to concatenate")]
        public string FileExtension { get; set; }

        [CommandArgument(1, "[PATH]")]
        [Description("Path where to find the files to concatenate")]
        public string Path { get; set; } = ".";

        [CommandArgument(1, "[OUTPUT]")]
        [Description("Path where to find the files to concatenate")]
        public string Output { get; set; } = "ConcatOutput.txt";
    }
}