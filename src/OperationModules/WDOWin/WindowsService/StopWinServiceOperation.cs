using System.ServiceProcess;
using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOWin.WindowsService;

public class StopWinServiceOperation : BaseOperation
{
    [JsonProperty]
    public string ServiceName { get; init; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing StopWinServiceOperation...");
        try
        {
            var sc = new ServiceController(ServiceName);
            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error stopping service: {Message}", e.Message);
            return new ErrorResult($"Error stopping service: {e.Message}");
        }
    }


    public void Deconstruct(out string ServiceName)
    {
        ServiceName = this.ServiceName;
    }
}