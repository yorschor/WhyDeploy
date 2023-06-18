using Newtonsoft.Json;
using NLog;
using WDBase;
using WDBase.Collections;
using WDBase.Models;
using WDCore.Serialisation;
using WDUtility;

namespace WDCore.StorageHelper;

// ReSharper disable once InconsistentNaming
public static class WDAppStorageHelper
{
    /// <summary>
    /// The default name for the centralized app storage.
    /// An app storage ist simply a json file containing a List of DeployAppReference(s)
    /// </summary>
    public const string appConfigName = "WhyDeployAppStorage.json";

    /// <summary>
    /// Try to parse a path to a List of DeployAppReference(s)
    /// </summary>
    /// <param name="storagePath">Path to the AppStorage json file</param>
    /// <returns></returns>
    public static Result<List<DeployAppReference>> GetAppStorage(string storagePath)
    {
        if (string.IsNullOrEmpty(storagePath) || !File.Exists(storagePath))
            return new ErrorResult<List<DeployAppReference>>(
                $"No app storage found at {storagePath}. File does not exist!");

        try
        {
            var jsonString = File.ReadAllText(storagePath);
            var apps = JsonConvert.DeserializeObject<List<DeployAppReference>>(jsonString);
            if (apps == null)
                return new ErrorResult<List<DeployAppReference>>(
                    $"No app storage found at {storagePath}. File is invalid!");

            return new SuccessResult<List<DeployAppReference>>(apps);
        }
        catch (Exception e)
        {
            return new ErrorResult<List<DeployAppReference>>($"Error reading apps from storage: {e.Message}");
        }
    }

    /// <summary>
    /// Tries to add a DeployAppReference to an existing AppStorage
    /// </summary>
    /// <param name="newApp">The app reference that should be added to the storage</param>
    /// <param name="storagePath">Path to the AppStorage json file</param>
    /// <returns></returns>
    public static Result AddAppToStorage(DeployAppReference newApp, string storagePath)
    {
        try
        {
            var storageResult = GetAppStorage(storagePath);
            if (storageResult is IErrorResult e)
                return new ErrorResult(e.Message);

            var storage = storageResult.Data;
            storage.Add(newApp);
            var jsonString = JsonConvert.SerializeObject(storage);
            File.WriteAllText(storagePath, jsonString);

            return new SuccessResult();
        }
        catch (Exception e)
        {
            return new ErrorResult($"Something went wrong while trying to Add new App to storage : {e.Message}");
        }
    }
    
    public static Result RemoveAppsFromStorage(IEnumerable<DeployAppReference> appsToRemove, string storagePath)
    {
        try
        {
            var storageResult = GetAppStorage(storagePath);
            if (storageResult is IErrorResult e)
                return new ErrorResult(e.Message);

            var storage = storageResult.Data;
            foreach (var app in appsToRemove)
            {
                storage.Remove(app);

            }
            var jsonString = JsonConvert.SerializeObject(storage);
            File.WriteAllText(storagePath, jsonString);

            return new SuccessResult();
        }
        catch (Exception e)
        {
            return new ErrorResult($"Something went wrong while trying to Add new App to storage : {e.Message}");
        }
    }

    /// <summary>
    /// Attempts to parse a given file to a DeployAppReference
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Result<DeployApp> TryGetAppConfigFromPath(string path)
    {
        try
        {
            var jsonString = File.ReadAllText(path);
            var app = JsonConvert.DeserializeObject<DeployApp>(jsonString);
            return new SuccessResult<DeployApp>(app);
        }
        catch (Exception e)
        {
            return new ErrorResult<DeployApp>($"Error getting app reference from path: {e.Message}");
        }
    }

    /// <summary>
    /// Get a DeployAppReference from storage 
    /// </summary>
    /// <param name="storagePath">Path to the AppStorage json file</param>
    /// <param name="name"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static Result<DeployAppReference> GetAppRefByName(string storagePath, string name, ILogger logger)
    {
        try
        {
            var appsResult = GetAppStorage(storagePath);
            if (appsResult is IErrorResult e)
                return new ErrorResult<DeployAppReference>(e.Message);

            return new SuccessResult<DeployAppReference>(appsResult.Data.FirstOrDefault(x => x.Name == name));
        }
        catch (Exception e)
        {
            return new ErrorResult<DeployAppReference>($"Error retrieving app with name {name}: {e.Message}");
        }
    }

    public static Result<string> CreateNewAppStorage(string path)
    {
        try
        {
            var finalPath = Path.Combine(path, appConfigName);
            if (File.Exists(finalPath)) return new ErrorResult<string>("AppStorage already exists at " + finalPath);

            var newAppConfig = JsonConvert.SerializeObject(new List<DeployAppReference>());
            FileSystemHelper.WriteToFileWithPathInsurance(finalPath, newAppConfig);
            return new SuccessResult<string>(finalPath);
        }
        catch (Exception e)
        {
            return new ErrorResult<string>(e.Message);
        }
    }

    public static Result<DeployAppReference> GenerateAppRefFromConfigPath(string path)
    {
        var configResult = TryGetAppConfigFromPath(path);
        if (configResult is not IErrorResult err)
        {
            return new SuccessResult<DeployAppReference>(new DeployAppReference(configResult.Data.Name, path, configResult.Data.AppId));
        }

        return new ErrorResult<DeployAppReference>(err.Message, err.Errors);
    }

    private static Result<DeployApp> ConvertAppRefToConfig(DeployAppReference appReference)
    {
        var appConfigResult = TryGetAppConfigFromPath(appReference.PathToApp);
        return appConfigResult switch
        {
            ErrorResult<DeployApp> err => err,
            SuccessResult<DeployApp> success => success,
            _ => new ErrorResult<DeployApp>("Impossible to reach!")
        };
    }

    public static Result<DeployApp> GetAppConfigByName(string storagePath, string appName, ILogger logger)
    {
        try
        {
            var appRef = GetAppRefByName(storagePath, appName, logger);
            return ConvertAppRefToConfig(appRef.Data);
        }
        catch (Exception e)
        {
            return new ErrorResult<DeployApp>(e.Message);
        }
    }

    public static Result RevalidateStorage(string storagePath)
    {
        var appStorageResult = GetAppStorage(storagePath);
        if (appStorageResult is IErrorResult result) return new ErrorResult(result.Message, result.Errors);
        var storage = appStorageResult.Data;
        storage = storage.Distinct().ToList();
        
        var appsToRemove =  new List<DeployAppReference>();
        foreach (var app in storage)
        {
            if (!Path.Exists(app.PathToApp)) appsToRemove.Add(app);
            else
            {
                var actualAppResult = TryGetAppConfigFromPath(app.PathToApp);
                if (actualAppResult is IErrorResult err)
                {
                    appsToRemove.Add(app);
                    continue;
                }
                if (app.Id != actualAppResult.Data.AppId) appsToRemove.Add(app);
                
            }
        }

        foreach (var app in appsToRemove)
        {
            storage.Remove(app);
        }
        
        var jsonString = JsonConvert.SerializeObject(storage);
        File.WriteAllText(storagePath, jsonString);
        

        return new SuccessResult();
    }
}