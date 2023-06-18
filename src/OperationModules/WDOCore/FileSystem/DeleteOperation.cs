using Newtonsoft.Json;
using WDBase;
using WDBase.Models;
using WDUtility;

namespace WDOCore.FileSystem;

[JsonObject]
public class DeleteOperation : BaseOperation
{
    [JsonProperty]
    public string Target { get; init; } = string.Empty;

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing DeleteOperation...");
        Logger.Info("Target: {Target}", FileSystemHelper.CombinePaths(context.BaseTargetContext, Target));
        try
        {
            Do(this, context);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing DeleteOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing DeleteOperation: {e.Message}");
        }
    }


    public static void Do(DeleteOperation op, LocationContext context)
    {
        FileSystemHelper.Delete(FileSystemHelper.CombinePaths(context.BaseTargetContext, op.Target));
    }

    public void Deconstruct(out string Target)
    {
        Target = this.Target;
    }
}