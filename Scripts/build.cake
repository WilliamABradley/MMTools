#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var baseDir = MakeAbsolute(Directory("../")).ToString();
var currentDir = MakeAbsolute(Directory("./")).ToString();

// FFTools Library
var libraryPath = baseDir + "/FFTools/FFTools.csproj";

// Executable Packaging
var baseSpecPath = currentDir + "/NativeNugetSpec.nuspec";
var baseTargetPath = currentDir + "/NativeNuget.targets";

var srcDir = currentDir + "/bin";
var outDir = currentDir + "/output";
var publishDir = currentDir + "/publish";

//////////////////////////////////////////////////////////////////////
// FFTOOLS EXECUTABLES CONFIG
//////////////////////////////////////////////////////////////////////

string Version = "4.1";
string WindowsVersion = Version;
string LinuxVersion = Version;
string OSXVersion = "4.1.7";

var Runtimes = new []{
    new {
        repoName = "win-32",
        runtimeName = "win-x86",
        version = WindowsVersion,
        os = "Windows",
        arch = "X86"
    },
    new {
        repoName = "win-64",
        runtimeName = "win-x64",
        version = WindowsVersion,
        os = "Windows",
        arch = "X64"
    },
    new {
        repoName = "linux-32",
        runtimeName = "linux-x86",
        version = LinuxVersion,
        os = "Linux",
        arch = "X86"
    },
    new {
        repoName = "linux-64",
        runtimeName = "linux-x64",
        version = LinuxVersion,
        os = "Linux",
        arch = "X64"
    },
    new {
        repoName = "osx-64",
        runtimeName = "osx-x64",
        version = OSXVersion,
        os = "MacOS",
        arch = "X64"
    }
};

string[] Applications = new string[]{
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
        MaxCpuCount = 0
    }
    .SetConfiguration("Release")
    .WithTarget("Restore;Pack")
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

    foreach (var app in Applications)
    {
        foreach(var runtime in Runtimes)
        {
            Information($"Downloading {app} {Version} {runtime.runtimeName}");
            var url = $"https://github.com/vot/ffbinaries-prebuilt/releases/download/v{Version}/{app}-{runtime.version}-{runtime.repoName}.zip";
            var file = DownloadFile(url);
            Information($"Downloaded {app} {Version} {runtime.runtimeName}");

            var destPath = $"{outDir}/runtimes/{runtime.runtimeName}/FFMPEG";
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

    foreach(var runtime in Runtimes)
    {
        var nugetID = $"FFTools.Executables.{runtime.os}.{runtime.arch}";
        var nugetSrcPath = $"runtimes/{runtime.runtimeName}/FFMPEG";
        var filesPath = $"{outDir}/{nugetSrcPath}/**";
        var targetDest = $"build/{nugetID}.targets";

        var nuGetPackSettings   = new NuGetPackSettings {
            Id                       = nugetID,
            Version                  = Version,
            Title                    = nugetID,
            Summary                  = $"FFMPEG Executables for FFTools on {runtime.os} {runtime.arch}",
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
