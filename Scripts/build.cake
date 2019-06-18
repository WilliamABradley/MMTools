#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var baseDir = MakeAbsolute(Directory("../")).ToString();
var currentDir = MakeAbsolute(Directory("./")).ToString();

// MMTools Library
string PkgVersion = "1.0.4";
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

string FFToolsVersion = "4.1";
string FFWindowsVersion = FFToolsVersion;
string FFLinuxVersion = FFToolsVersion;
string FFOSXVersion = "4.1.7";

var FFRuntimes = new []{
    new {
        repoName = "win-32",
        runtimeName = "win-x86",
        version = FFWindowsVersion,
        os = "Windows",
        arch = "X86"
    },
    new {
        repoName = "win-64",
        runtimeName = "win-x64",
        version = FFWindowsVersion,
        os = "Windows",
        arch = "X64"
    },
    new {
        repoName = "linux-32",
        runtimeName = "linux-x86",
        version = FFLinuxVersion,
        os = "Linux",
        arch = "X86"
    },
    new {
        repoName = "linux-64",
        runtimeName = "linux-x64",
        version = FFLinuxVersion,
        os = "Linux",
        arch = "X64"
    },
    new {
        repoName = "osx-64",
        runtimeName = "osx-x64",
        version = FFOSXVersion,
        os = "MacOS",
        arch = "X64"
    }
};

string[] FFApplications = new string[]{
    "ffmpeg",
    "ffprobe"
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
    CleanDirectory(outDir);

    foreach (var app in FFApplications)
    {
        foreach(var runtime in FFRuntimes)
        {
            Information($"Downloading {app} {FFToolsVersion} {runtime.runtimeName}");
            var url = $"https://github.com/vot/ffbinaries-prebuilt/releases/download/v{FFToolsVersion}/{app}-{runtime.version}-{runtime.repoName}.zip";
            var file = DownloadFile(url);
            Information($"Downloaded {app} {FFToolsVersion} {runtime.runtimeName}");

            var destPath = $"{outDir}/runtimes/{runtime.runtimeName}/MMTools";
            Unzip(file, destPath);
            
            // Clean up crap the OSX left behind.
            var macOSFolder = $"{destPath}/__MACOSX";
            if(DirectoryExists(macOSFolder)){
                DeleteDirectory(macOSFolder, new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                });
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
        var nugetSrcPath = $"runtimes/{runtime.runtimeName}/MMTools";
        var filesPath = $"{outDir}/{nugetSrcPath}/**";
        var targetDest = $"build/{nugetID}.targets";

        var nuGetPackSettings   = new NuGetPackSettings {
            Id                       = nugetID,
            Version                  = PkgVersion,
            Title                    = nugetID,
            Summary                  = $"Executables for MMTools on {runtime.os} {runtime.arch}",
            Symbols                  = false,
            NoPackageAnalysis        = true,
            Files                    = new [] {
                new NuSpecContent { Source = filesPath },
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
