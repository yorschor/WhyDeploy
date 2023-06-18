using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOCore;

[JsonObject]
public class WaitOperation : BaseOperation
{
    [JsonProperty]
    public float TimeSeconds { get; init; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing WaitOperation...");
        Logger.Info("TimeSeconds: {TimeSeconds}", TimeSeconds);
        try
        {
            Do(this, context);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error executing WaitOperation: {Message}", e.Message);
            return new ErrorResult($"Error executing WaitOperation: {e.Message}");
        }
    }


    public static void Do(WaitOperation op, LocationContext context)
    {
        Thread.Sleep(TimeSpan.FromSeconds(op.TimeSeconds));
    }

    public void Deconstruct(out float timeSeconds)
    {
        timeSeconds = TimeSeconds;
    }
}