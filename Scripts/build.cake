#addin nuget:https://api.nuget.org/v3/index.json?package=Cake.FileHelpers&version=5.0.0

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var baseDir = MakeAbsolute(Directory("../")).ToString();
var currentDir = MakeAbsolute(Directory("./")).ToString();
var certName = Argument("cert", "Developer ID Application: *");

// MMTools Library
string PkgVersion = "1.0.9";
var libraryPath = baseDir + "/MMTools/MMTools.csproj";

// Executable Packaging
var baseSpecPath = currentDir + "/NativeNugetSpec.nuspec";
var baseTargetPath = currentDir + "/NativeNuget.targets";

var srcDir = currentDir + "/bin";
var outDir = currentDir + "/output";
var publishDir = currentDir + "/publish";

//////////////////////////////////////////////////////////////////////
// FFMPEG EXECUTABLES CONFIG
//////////////////////////////////////////////////////////////////////

string FFToolsVersion = "5.0";
string FFWindowsVersion = FFToolsVersion;
string FFLinuxVersion = FFToolsVersion;
string FFOSXVersion = FFToolsVersion;

var FFRuntimes = new []{
    new {
        repoName = "win32-ia32",
        runtimeName = "win-x86",
        version = FFWindowsVersion,
        os = "Windows",
        arch = "X86"
    },
    new {
        repoName = "win32-x64",
        runtimeName = "win-x64",
        version = FFWindowsVersion,
        os = "Windows",
        arch = "X64"
    },
    new {
        repoName = "linux-ia32",
        runtimeName = "linux-x86",
        version = FFLinuxVersion,
        os = "Linux",
        arch = "X86"
    },
    new {
        repoName = "linux-x64",
        runtimeName = "linux-x64",
        version = FFLinuxVersion,
        os = "Linux",
        arch = "X64"
    },
    new {
        repoName = "linux-arm",
        runtimeName = "linux-arm",
        version = FFLinuxVersion,
        os = "Linux",
        arch = "ARM32"
    },
    new {
        repoName = "linux-arm64",
        runtimeName = "linux-arm64",
        version = FFLinuxVersion,
        os = "Linux",
        arch = "ARM64"
    },
    new {
        repoName = "darwin-x64",
        runtimeName = "osx-x64",
        version = FFOSXVersion,
        os = "MacOS",
        arch = "X64"
    },
    new {
        repoName = "darwin-arm64",
        runtimeName = "osx-arm64",
        version = FFOSXVersion,
        os = "MacOS",
        arch = "ARM"
    }
};

string[] FFApplications = new string[]{
    "ffmpeg"
};

//////////////////////////////////////////////////////////////////////
// DEFAULT TASK
//////////////////////////////////////////////////////////////////////

Task("CleanPublish")
    .Description("Cleans the Destination folder")
    .Does(() =>
{
    EnsureDirectoryExists(publishDir);
    CleanDirectory(publishDir);
});

Task("Build")
    .Description("Builds the Library")
    .IsDependentOn("CleanPublish")
    .Does(() =>
{
    EnsureDirectoryExists(srcDir);
    CleanDirectory(srcDir);

    Information($"\nBuilding {libraryPath}");
    var buildSettings = new MSBuildSettings
    {
        MaxCpuCount = 0,
        ToolVersion = MSBuildToolVersion.VS2019
    }
    .SetConfiguration("Release")
    .WithTarget("Restore;Pack")
    .WithProperty("Version", PkgVersion)
    .WithProperty("GenerateLibraryLayout", "true")
    .WithProperty("OutputPath", srcDir);

    MSBuild(libraryPath, buildSettings);

    var buildPackages = $"{srcDir}/**/*.nupkg"; 
    CopyFiles(buildPackages, publishDir);
});

Task("CollectExecutables")
    .Description("Gathers the Executables from the Sources")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(outDir);
    //CleanDirectory(outDir);

    foreach (var app in FFApplications)
    {
        foreach(var runtime in FFRuntimes)
        {
            Information($"Downloading {app} {FFToolsVersion} {runtime.runtimeName}");
            // https://github.com/eugeneware/ffmpeg-static/releases/download/b5.0/darwin-arm64
            var url = $"https://github.com/eugeneware/ffmpeg-static/releases/download/b{FFToolsVersion}/{runtime.repoName}";
            
            var destFolder = new DirectoryPath($"{outDir}/runtimes/{runtime.runtimeName}/MMTools");
            EnsureDirectoryExists(destFolder);

            var filename = "ffmpeg";
            if (runtime.runtimeName.Contains("win"))
                filename = filename + ".exe";
            var destPath = destFolder.GetFilePath(filename);

            if (FileExists(destPath))
            {
                Information($"Found {destPath}, will not download anything");
            }
            else
            {
                DownloadFile(url, destPath);
                Information($"Downloaded {url} to {destPath}");
            }

            using(var process = StartAndReturnProcess("chmod", new ProcessSettings {
                Silent = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                Arguments = new ProcessArgumentBuilder()
                    .Append("+x")
                    .Append("\"" + destPath.FullPath + "\"")
                }
            )){
                process.WaitForExit();
            }

            if (runtime.runtimeName.Contains("osx"))
            {
                using(var process = StartAndReturnProcess("codesign", new ProcessSettings {
                    Silent = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    Arguments = new ProcessArgumentBuilder()
                        .Append("--verbose=4")
                        .Append("--deep")
                        .Append("--force")
                        .Append("-s")
                        .Append("\"" + certName + "\"")
                        .Append("\"" + destPath.FullPath + "\"")
                    }
                )){
                    process.WaitForExit();
                }
            }
        }
    }
});

Task("PackageExecutables")
    .Description("Creates Native Nuget Packages from Executables")
    .IsDependentOn("CollectExecutables")
    .Does(() =>
{
    Information("Packing to " + publishDir);

    foreach(var runtime in FFRuntimes)
    {
        var nugetID = $"MMTools.Executables.{runtime.os}.{runtime.arch}";
        var nugetSrcPath = $"runtimes/{runtime.runtimeName}/MMTools/ffmpeg";

        if (runtime.runtimeName.Contains("win"))
        {
            nugetSrcPath = nugetSrcPath + ".exe";
        }

        var filesPath = $"{outDir}/{nugetSrcPath}";
        var targetDest = $"build/{nugetID}.targets";

        var nuGetPackSettings   = new NuGetPackSettings {
            Id                       = nugetID,
            Version                  = PkgVersion,
            Title                    = nugetID,
            Summary                  = $"Executables for MMTools on {runtime.os} {runtime.arch}",
            Symbols                  = false,
            NoPackageAnalysis        = true,
            Files                    = new [] {
                new NuSpecContent { Source = filesPath, Target = nugetSrcPath },
                new NuSpecContent { Source = baseTargetPath, Target = targetDest },
            },
            BasePath                 = outDir,
            OutputDirectory          = publishDir
        };

        NuGetPack(baseSpecPath, nuGetPackSettings);
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("PackageExecutables");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
