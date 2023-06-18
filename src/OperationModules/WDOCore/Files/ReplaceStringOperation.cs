using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOCore.Files;

[JsonObject]
public class ReplaceStringOperation : BaseOperation
{
    [JsonProperty]
    public string Target { get; init; } = string.Empty;

    [JsonProperty]
    public string OldString { get; init; } = string.Empty;

    [JsonProperty]
    public string NewString { get; init; } = string.Empty;

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing ReplaceStringOperation...");
        Logger.Info("Target: {Target}", Path.Combine(context.BaseSourceContext, Target));
        Logger.Info("OldString: {OldString}", OldString);
        Logger.Info("NewString: {NewString}", NewString);
        try
        {
            var path = Path.Combine(context.BaseTargetContext, Target);
            var text = File.ReadAllText(path);
            text = text.Replace(OldString, NewString);
            File.WriteAllText(path, text);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing ReplaceStringOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing ReplaceStringOperation: {e.Message}");
        }
    }


    public void Deconstruct(out string Target, out string OldString, out string NewString)
    {
        Target = this.Target;
        OldString = this.OldString;
        NewString = this.NewString;
    }
}