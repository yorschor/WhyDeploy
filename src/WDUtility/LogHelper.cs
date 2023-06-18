using NLog;

namespace WDUtility;

public static class LogHelper
{
    public static bool FileExists(string path, ILogger logger)
    {
        if (File.Exists(path)) return true;
        logger.Warn("File {FilePath} doesn not exist", path);
        return false;
    }

    public static bool DirectoryExists(string path, ILogger logger)
    {
        if (Directory.Exists(path)) return true;
        logger.Warn("Directory {DirPath} doesn not exist", path);
        return false;
    }
}