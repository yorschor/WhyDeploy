using System.Reflection;
using NLog;
using WDBase;

namespace WDCore;

public class ModuleLoader
{
    private readonly string _moduleFolder;

    public ModuleLoader(string moduleFolder)
    {
        _moduleFolder = moduleFolder;
    }

    public IEnumerable<BaseOperation> LoadModules()
    {
        var modules = new List<BaseOperation>();

        var dllFileNames = Array.Empty<string>();

        if(Directory.Exists(_moduleFolder))
        {
            dllFileNames = Directory.GetFiles(_moduleFolder, "*.dll");
        }

        if (dllFileNames.Length < 1) return modules;
        foreach (var dllFile in dllFileNames)
        {
            Type[] moduleTypes = null;
            var fullPathDllFile = Path.GetFullPath(dllFile);

            try
            {
                var moduleAssembly = Assembly.LoadFile(fullPathDllFile);
                if (moduleAssembly != null)
                {
                    moduleTypes = moduleAssembly.GetTypes();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
            }

            if (moduleTypes == null) continue;
            foreach (var type in moduleTypes)
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseOperation)))
                {
                    LogManager.GetCurrentClassLogger().Info($"Loaded Operation: {type.Name}");
                    // try
                    // {
                    //     var module = (BaseOperation)Activator.CreateInstance(type);
                    //     modules.Add(module);
                    // }
                    // catch (Exception ex)
                    // {
                    //     // Log or handle exception during module instantiation
                    // }
                }
            }
        }

        return modules;
    }
}