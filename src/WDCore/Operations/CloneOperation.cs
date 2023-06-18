using Newtonsoft.Json;
using WDBase;
using WDBase.Models;
using WDUtility;

namespace WDCore.Operations;

[JsonObject]
public class CloneOperation : BaseOperation
{
    [JsonProperty]
    public string To { get; init; } = string.Empty;

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing CloneOperation...");
        Logger.Info("From:  {Source}", context.BaseSourceContext);
        Logger.Info("To: {Target}", Path.Combine(context.BaseTargetContext, To));
        try
        {
            Do(this, context);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing CloneOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing CloneOperation: {e.Message}");
        }
    }


    public static void Do(CloneOperation op, LocationContext context)
    {
        FileSystemHelper.Copy(context.BaseSourceContext, Path.Combine(context.BaseTargetContext, op.To), true);
    }

    public void Deconstruct(out string To)
    {
        To = this.To;
    }
}