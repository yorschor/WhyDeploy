using Spectre.Console;
using WDBase;

namespace WhyDeployCLI.CLIExtension;

public static class TreeHelper
{
    public static void InsertOperationsTreeArray(TreeNode root, BaseOperation[] operations)
    {
        foreach (var operation in operations) InsertOperationTree(root, operation);
    }

    public static void InsertOperationTree(TreeNode root, BaseOperation operation)
    {
        var subnode = root.AddNode($"[italic bold silver]{operation.OperationName}[/]");
        var properties = operation.GetOperationProperties();
        foreach (var prop in properties) subnode.AddNode($"* [bold deepskyblue1]{prop.Key}[/] -> {prop.Value}");
    }
}