using System.ServiceProcess;
using Newtonsoft.Json;
using WDBase;
using WDBase.Models;

namespace WDOWin.WindowsService;

public class StartWinServiceOperation : BaseOperation
{
    [JsonProperty]
    public string ServiceName { get; init; }

    public override Result Execute(LocationContext context)
    {
        Logger.Info("Executing StartWinServiceOperation...");
        try
        {
            var sc = new ServiceController(ServiceName);
            sc.Start();
            sc.WaitForStatus(ServiceControllerStatus.Running);
            return new SuccessResult();
        }
        catch (Exception e)
        {
            Logger.Error("Error starting service: {Message}", e.Message);
            return new ErrorResult($"Error starting service: {e.Message}");
        }
    }


    public void Deconstruct(out string ServiceName)
    {
        ServiceName = this.ServiceName;
    }
}