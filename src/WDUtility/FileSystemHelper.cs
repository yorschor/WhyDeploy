// ReSharper disable MemberCanBePrivate.Global

using System.Text;
using NLog;

namespace WDUtility;

public static class FileSystemHelper
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    #region Public

    /// <summary>
    ///     Copies ether a file or a directory from source to destination
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="recursive"></param>
    public static void Copy(string source, string destination, bool recursive)
    {
        if (Path.Exists(source))
        {
            if (IsFile(source))
            {
                CopyFile(source, destination, true);
                Thread.Sleep(100);

                if (!File.Exists(destination))
                    throw new FileNotFoundException($"Destination file not found after copy: {destination}");
            }
            else if (IsDirectory(source))
            {
                CopyDirectory(source, destination, recursive);
                Thread.Sleep(100);

                if (!Directory.Exists(destination))
                    throw new DirectoryNotFoundException($"Destination directory not found after copy: {destination}");
            }
        }
        else
        {
            throw new ArgumentException($"{source} is not a valid file or directory");
        }
    }

    /// <summary>
    ///     Deletes ether the specified file or directory
    /// </summary>
    /// <param name="target"></param>
    /// <param name="failOnNonExistingPath">If set to true the function will log to Console that the Path is nat a valid one</param>
    public static void Delete(string target, bool failOnNonExistingPath = false)
    {
        if (Path.Exists(target))
        {
            if (IsFile(target))
                File.Delete(target);
            else Directory.Delete(target, true);
        }
        else if (failOnNonExistingPath)
        {
            throw new ArgumentException($"{target} is not a valid file or directory.");
        }
    }

    public static bool IsFile(string path)
    {
        return File.Exists(path);
    }

    public static bool IsDirectory(string path)
    {
        return Directory.Exists(path);
    }

    public static void WriteToFileWithPathInsurance(string path, string? contents)
    {
        CreateDirectoryIfDoesntExist(Path.GetDirectoryName(path));
        File.WriteAllText(path, contents);
    }

    public static void CreateDirectoryIfDoesntExist(string? path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path!);
    }

    /// <summary>
    ///     Searches for files with a specified name within a specified directory and its subdirectories.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to search in.</param>
    /// <param name="targetFileName">The name of the files to search for.</param>
    /// <returns>An enumerable collection of file paths that match the specified name.</returns>
    public static IEnumerable<string> FindFilesByName(string directoryPath, string targetFileName)
    {
        var result = Enumerable.Empty<string>();
        try
        {
            result = Directory.EnumerateFiles(directoryPath, targetFileName, SearchOption.AllDirectories);
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
            throw;
        }

        return result;
    }


    /// <summary>
    ///     Searches for directories with a specified name within a specified directory and its subdirectories.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to search in.</param>
    /// <param name="targetDirectoryName">The name of the directories to search for.</param>
    /// <returns>An enumerable collection of directory paths that match the specified name.</returns>
    public static IEnumerable<string> FindDirectoriesByName(string directoryPath, string targetDirectoryName)
    {
        var result = Enumerable.Empty<string>();
        try
        {
            result = Directory.EnumerateDirectories(directoryPath, "*", SearchOption.AllDirectories)
                .Where(dir => string.Equals(Path.GetFileName(dir), targetDirectoryName,
                    StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            Logger.Error(ex.Message);
            throw;
        }

        return result;
    }


    //TODO Move to windows specific util project
    // /// <summary>
    // ///     Create a windows shortcut
    // /// </summary>
    // /// <param name="shortcutName"></param>
    // /// <param name="shortcutTargetPath"></param>
    // /// <param name="saveLocation"></param>
    // /// <param name="workingDirectory"></param>
    // /// <param name="description"></param>
    // /// <param name="iconPath"></param>
    // public static void CreateShortcut(string shortcutName, string shortcutTargetPath, string saveLocation = "",
    //     string workingDirectory = "", string description = "", string? iconPath = null)
    // {
    //     using var shortcut = new WindowsShortcut();
    //     shortcut.Path = shortcutTargetPath;
    //     shortcut.WorkingDirectory = workingDirectory;
    //     shortcut.Description = description;
    //     shortcut.IconLocation = iconPath ?? string.Empty;
    //     var saveLoc = string.IsNullOrEmpty(saveLocation) ? Directory.GetCurrentDirectory() : saveLocation;
    //     shortcut.Save(Path.Combine(saveLoc, $"{shortcutName}.lnk"));
    // }

    /// <summary>
    ///     Concatenates all files with a specific extension in a directory and writes them into a new file.
    /// </summary>
    /// <param name="sourceDirectory">The source directory containing the files to concatenate.</param>
    /// <param name="fileExtension">The file extension to filter the files to concatenate.</param>
    /// <param name="destinationFile">The path to the destination file where the concatenated content will be written.</param>
    public static void ConcatenateFiles(string sourceDirectory, string fileExtension, string destinationFile)
    {
        if (!IsDirectory(sourceDirectory))
        {
            Logger.Warn($"{sourceDirectory} is not a valid directory");
            return;
        }

        // Get all files and sort them alphabetically
        var filesToConcatenate = Directory
            .EnumerateFiles(sourceDirectory, $"*.{fileExtension}", SearchOption.AllDirectories)
            .OrderBy(file => file) // Sorting the files alphabetically
            .ToList();

        using var outputStream = new FileStream(destinationFile, FileMode.Append);

        foreach (var file in filesToConcatenate)
        {
            var fileContent = File.ReadAllText(file);
            // Normalize line endings to \n (or \r\n if you prefer)
            fileContent = fileContent.Replace("\r\n", "\n").Replace("\r", "\n");

            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            inputStream.CopyTo(outputStream);
        }
    }


    public static string CombinePaths(params string[] paths)
    {
        if (paths.Length == 0) return string.Empty;

        // Start with the first path
        var result = paths[0];

        for (var i = 1; i < paths.Length; i++)
        {
            var path = paths[i];

            if (string.IsNullOrEmpty(path))
                continue;

            // If the next path is an absolute path, then use it directly.
            if (Path.IsPathRooted(path))
            {
                result = path;
                continue;
            }

            // Identify whether the paths are Windows or Linux paths
            var isResultWindowsPath = result.Contains('\\');

            var isPathWindowsPath = path.Contains('\\');

            var separator = isResultWindowsPath switch
            {
                // Decide the separator based on the type of paths being combined
                true when isPathWindowsPath => '\\',
                false when !isPathWindowsPath => '/',
                _ => Path.DirectorySeparatorChar
            };

            // Combine the paths using the selected separator
            result = result.TrimEnd('/', '\\') + separator + path.TrimStart('/', '\\');
        }

        return result;
    }

    #endregion

    #region Private Util

    private static void CopyFile(string sourceFile, string destinationFile, bool overwrite)
    {
        var destinationDir = Path.GetDirectoryName(destinationFile);

        if (destinationDir != null) CreateDirectoryIfDoesntExist(destinationDir);

        File.Copy(sourceFile, destinationFile, overwrite);
    }

    private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            File.Copy(file.FullName, targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (!recursive) return;
        foreach (var subDir in dirs)
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true);
        }
    }

    #endregion
}