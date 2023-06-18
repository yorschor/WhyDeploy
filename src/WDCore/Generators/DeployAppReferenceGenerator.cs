using WDBase.Models;

namespace WDCore.Generators;

public static class DeployAppReferenceGenerator
{
    public static DeployAppReference New(string location, DeployApp app)
    {
        return new DeployAppReference
        {
            Name = app.Name,
            PathToApp = location,
            Id = app.AppId
        };
    }
}