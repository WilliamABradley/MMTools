#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var baseDir = MakeAbsolute(Directory("../")).ToString();
var currentDir = MakeAbsolute(Directory("./")).ToString();
var baseSpecPath = currentDir + "/NativeNugetSpec.nuspec";

var outDir = currentDir + "/output/";
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

Task("Collect")
    .Description("Gathers the Executables from the Sources")
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

Task("Package")
    .Description("Creates Native Nuget Packages from Executables")
    //.IsDependentOn("Collect")
    .Does(() =>
{
    EnsureDirectoryExists(publishDir);
    CleanDirectory(publishDir);
    Information("Packing to " + publishDir);

    foreach(var runtime in Runtimes)
    {
        var nugetID = $"FFTools.Executables.{runtime.os}.{runtime.arch}";
        var nugetSrcPath = $"runtimes/{runtime.runtimeName}/FFMPEG";
        var filesPath = $"{outDir}{nugetSrcPath}/**";

        // Write file with full Path.
        //var fileInfo = $"{outDir}FF-EXE-{runtime.os}-{runtime.arch}";
        //FileWriteText(fileInfo, nugetSrcPath);

        var nuGetPackSettings   = new NuGetPackSettings {
            Id                       = nugetID,
            Version                  = Version,
            Title                    = nugetID,
            Summary                  = $"FFMPEG Executables for FFTools on {runtime.os} {runtime.arch}",
            Symbols                  = false,
            NoPackageAnalysis        = true,
            Files                    = new [] {
                new NuSpecContent { Source = filesPath },
                //new NuSpecContent { Source = fileInfo },
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
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
