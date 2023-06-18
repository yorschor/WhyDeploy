// namespace WhyDeployCLI;
//
// public class TreeHelpTextGenerator : DefaultHelpTextGenerator
// {
// 	protected override void GenerateCommands(CommandLineApplication application, TextWriter output,
// 		IReadOnlyList<CommandLineApplication> visibleCommands,
// 		int firstColumnWidth)
// 	{
// 		if (visibleCommands.Any())
// 		{
// 			output.WriteLine();
// 			output.WriteLine("Commands:");
// 			var outputFormat = $"  {{0, -{firstColumnWidth}}}{{1}}";
//
// 			var orderedCommands = SortCommandsByName
// 				? visibleCommands.OrderBy(c => c.Name).ToList()
// 				: visibleCommands;
//
// 			foreach (var cmd in orderedCommands)
// 			{
// 				var description = cmd.Description;
//
// 				var wrappedDescription = IndentWriter?.Write(description);
// 				var message = string.Format(outputFormat, cmd.Name, wrappedDescription);
//
// 				output.Write("├─");
// 				output.Write(message);
// 				output.WriteLine();
// 			}
//
// 			if (application.OptionHelp != null)
// 			{
// 				output.WriteLine();
// 				output.WriteLine(
// 					$"Run '{application.Name} [command] {Format(application.OptionHelp)}' for more information about a command.");
// 			}
// 		}
// 	}
// }

