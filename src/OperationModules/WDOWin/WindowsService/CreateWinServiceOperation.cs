using System.Diagnostics;
using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOWin.WindowsService;

public class CreateWinServiceOperation : BaseOperation
{
    [JsonProperty]
    public string ServiceName { get; set; }

    [JsonProperty]
    public string ServiceExecutablePath { get; set; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing CreateWinServiceOperation...");
        try
        {
            var psi = new ProcessStartInfo("sc", $"create {ServiceName} binPath= {ServiceExecutablePath}");
            var process = Process.Start(psi);
            process.WaitForExit();
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error installing service: {Message}", e.Message);
            return new ErrorResult($"Error installing service: {e.Message}");
        }
    }
}