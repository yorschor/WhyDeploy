using Newtonsoft.Json;
using WDBase;
using WDBase.Models;
using WDUtility;

namespace WDCore.Operations;

[JsonObject]
public class PurgeSelfOperation : BaseOperation
{
    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing PurgeSelfOperation...");
        Logger.Info("Target: {Target}", Path.Combine(context.BaseSourceContext));
        try
        {
            Do(this, context);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing PurgeSelfOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing PurgeSelfOperation: {e.Message}");
        }
    }


    public static void Do(PurgeSelfOperation op, LocationContext context)
    {
        FileSystemHelper.Delete(context.BaseTargetContext);
    }
}