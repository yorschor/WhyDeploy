using Newtonsoft.Json;

namespace WDBase.Models;

public record struct DeployAppReference(
    [JsonProperty] string Name,
    [JsonProperty] string PathToApp,
    [JsonProperty] string Id
)
{
    public override string ToString()
    {
        return Name + "\n" +
               " Path -> " + PathToApp;
    }
}