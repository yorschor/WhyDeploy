using System.Diagnostics;
using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOWin.WindowsService;

public class DeleteWinServiceOperation : BaseOperation
{
    [JsonProperty]
    public string ServiceName { get; set; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing DeleteWinServiceOperation...");
        try
        {
            var psi = new ProcessStartInfo("sc", $"delete {ServiceName}");
            var process = Process.Start(psi);
            process.WaitForExit();
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error deleting service: {Message}", e.Message);
            return new ErrorResult($"Error deleting service: {e.Message}");
        }
    }
}