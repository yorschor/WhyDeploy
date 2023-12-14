using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    #region Parameters

    [Solution] readonly Solution Solution;
    [Parameter("Target Framework", Name = "Framework")] readonly string TargetFramework = "net8.0";

    [Parameter("Runtime Identifier", Name = "Runtime")] readonly string RuntimeIdentifier = "win-x64";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("The version to use for the build")] readonly string Version = "0.42.0";
    [Parameter("The version suffix that should be appended to the version")] readonly string VersionSuffix = "";

    #endregion

    #region Properties

    AbsolutePath BuildPath => AbsolutePath.Create("") / "bin" / Configuration / TargetFramework / RuntimeIdentifier;
    AbsolutePath SourceDirectory => RootDirectory / "src" / "WhyDeployCLI" / BuildPath;
    AbsolutePath ModuleSourceDirectory => RootDirectory / "src" / "OperationModules";

    #endregion

    #region Common Targets

    Target Clean => target => target
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(s =>
                s.SetProject(Solution.Path));
        });

    Target Restore => target => target
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(Solution.Path));
        });

    Target Compile => target => target
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetConfiguration(Configuration)
                .SetFramework(TargetFramework));
        });

    #endregion

    #region Helper
    void BuildModules(IEnumerable<string> modules)
    {
        foreach (var moduleName in modules)
        {
            Project project;
            try
            {
                project = Solution.AllProjects.First(x => x.Name == moduleName);
            }
            catch
            {
                continue;
            }
            DotNetTasks.DotNetPublish(s => s
                .SetProject(project.Path)
                .SetConfiguration(Configuration)
                .SetFramework(TargetFramework)
                .SetRuntime(RuntimeIdentifier)
                .SetVersionPrefix(Version)
                .SetVersionSuffix(VersionSuffix));
        }
    }

    void CopyWDDlls(AbsolutePath dllSourceDir, AbsolutePath destinationDir)
    {
        foreach (var dllName in dllSourceDir.GetFiles("WD*.dll"))
        {
            CopyFileToDirectory(dllSourceDir / dllName, destinationDir, FileExistsPolicy.Overwrite);
        } 
    }
    void CopyModules(IEnumerable<string> modules, AbsolutePath moduleSourceDir, AbsolutePath destinationDir)
    {
        foreach (var moduleName in modules)
        {
            CopyFileToDirectory(moduleSourceDir / moduleName / "bin" / Configuration / TargetFramework / RuntimeIdentifier / $"{moduleName}.dll", destinationDir, FileExistsPolicy.Overwrite);
        }
    }

    #endregion

    #region LocalDeploy

    AbsolutePath LocalDeployDirectory => RootDirectory / "Testbed" / "active";
    AbsolutePath LocalModulesDirectory => LocalDeployDirectory / "modules";

    IEnumerable<string> LocalModulesToCopy => new[]
    {
        "WDOCore",
        "WDOGit",
    };

    Target DeployCliLocal => t => t
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution.GetProject("WhyDeployCLI")?.Path)
                .SetConfiguration(Configuration.Debug)
                .SetFramework(TargetFramework)
                .SetRuntime(RuntimeIdentifier)
                .SetPublishSingleFile(true)
                .SetSelfContained(false)
                .SetPublishReadyToRun(false)
                .SetVersionPrefix(Version)
                .SetVersionSuffix(VersionSuffix)
                .SetOutputDirectory(LocalDeployDirectory));
            BuildModules(LocalModulesToCopy);
            CopyModules(LocalModulesToCopy, ModuleSourceDirectory, LocalModulesDirectory);
        });
    
    #endregion
    
    #region CreateRelease

    AbsolutePath OutDirectory => RootDirectory / "out";
    AbsolutePath DeployDirectory => OutDirectory / "active";
    AbsolutePath ModulesDirectory => DeployDirectory / "modules";

    IEnumerable<string> ModulesToCopy => new[]
    {
        "WDOCore",
        "WDOGit",
    };

    Target CreateCliRelease => t => t
        .Executes(() =>
        {
            OutDirectory.DeleteDirectory();
            DotNetTasks.DotNetPublish(s => s
                .SetProject(Solution.GetProject("WhyDeployCLI"))
                .SetConfiguration(Configuration.Release)
                .SetFramework(TargetFramework)
                .SetRuntime(RuntimeIdentifier)
                .SetPublishSingleFile(true)
                .SetSelfContained(true)
                .SetPublishReadyToRun(true)
                .SetVersionPrefix(Version)
                .SetVersionSuffix(VersionSuffix)
                .SetOutput(DeployDirectory));
            CopyModules(ModulesToCopy, ModuleSourceDirectory, ModulesDirectory);
            DeployDirectory.GlobFiles("**.pdb").DeleteFiles();
            ZipFile.CreateFromDirectory(DeployDirectory,
                OutDirectory / $"WhyDeployCLI_{Version + VersionSuffix}.zip",
                CompressionLevel.SmallestSize,
                false);
        });

    #endregion
}