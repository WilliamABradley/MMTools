using FFTools.Exceptions;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FFTools
{
    /// <summary>
    /// Configuration options for FFTools.
    /// </summary>
    public static class FFToolsConfiguration
    {
        /// <summary>
        /// Registers the Executable Directory for FFTools.
        /// </summary>
        /// <param name="directory">
        /// Directory of executables (Must be named just ffmpeg, ffprobe, etc. With/Without .exe on Windows).
        /// If not provided, will use the default directory from NuGet.
        /// </param>
        public static void RegisterExecutableDirectory(string directory = null)
        {
            if (directory == null)
            {
                directory = DefaultFFToolsDirectory();
            }

            _FFExecutableDirectory = directory;
            if (!Directory.Exists(_FFExecutableDirectory))
            {
                throw new FFExecutablesMissingException();
            }
        }

        /// <summary>
        /// Gets the Directory for FFMPEG executables for the Platform.
        /// </summary>
        /// <returns></returns>
        private static string DefaultFFToolsDirectory()
        {
            string OSPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OSPath += "win-";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OSPath += "osx-";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OSPath += "linux-";
            }

            if (OSPath == null)
            {
                throw new FFExecutablesMissingException();
            }

            OSPath += RuntimeInformation.OSArchitecture.ToString().ToLower();
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", OSPath, "FFMPEG");
        }

        /// <summary>
        /// The Directory for FFTools to use Executables.
        /// </summary>
        public static string FFExecutableDirectory
            => _FFExecutableDirectory ?? throw new FFExecutablesMissingException();

        /// <summary>
        /// Backing field for Executable Directory.
        /// </summary>
        private static string _FFExecutableDirectory;
    }
}
