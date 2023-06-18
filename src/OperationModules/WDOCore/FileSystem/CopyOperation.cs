using Newtonsoft.Json;
using WDBase;
using WDBase.Models;
using WDUtility;

namespace WDOCore.FileSystem;

[JsonObject]
public class CopyOperation : BaseOperation
{
    [JsonProperty]
    public string From { get; set; } = string.Empty;

    [JsonProperty]
    public string To { get; set; } = string.Empty;

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing CopyOperation...");
        Logger.Info("From: {Source}", Path.Combine(context.BaseSourceContext, From));
        Logger.Info("To: {Target}", Path.Combine(context.BaseTargetContext, To));
        try
        {
            Do(this, context);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing CopyOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing CopyOperation: {e.Message}");
        }
    }


    public static void Do(CopyOperation op, LocationContext context)
    {
        var from = Path.Combine(context.BaseSourceContext, op.From);
        var to = Path.Combine(context.BaseTargetContext, op.To);
        FileSystemHelper.Copy(from, to, true);
    }

    public void Deconstruct(out string From, out string To)
    {
        From = this.From;
        To = this.To;
    }
}