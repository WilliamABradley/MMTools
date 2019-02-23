using MMTools.Exceptions;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MMTools
{
    /// <summary>
    /// Configuration options for MMTools.
    /// </summary>
    public static class MMToolsConfiguration
    {
        /// <summary>
        /// Registers the Executable Directory for MMTools.
        /// </summary>
        /// <param name="directory">
        /// Directory of executables (Must be named just ffmpeg, ffprobe, etc. With/Without .exe on Windows).
        /// If not provided, will use the default directory from NuGet.
        /// </param>
        public static void RegisterExecutableDirectory(string directory = null)
        {
            if (directory == null)
            {
                directory = DefaultMMToolsDirectory();
            }

            _MMExecutableDirectory = directory;
            if (!Directory.Exists(_MMExecutableDirectory))
            {
                throw new MMExecutablesMissingException();
            }
        }

        /// <summary>
        /// Gets the Directory for FFMPEG executables for the Platform.
        /// </summary>
        /// <returns></returns>
        private static string DefaultMMToolsDirectory()
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
                throw new MMExecutablesMissingException();
            }

            OSPath += RuntimeInformation.OSArchitecture.ToString().ToLower();
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", OSPath, "MMTools");
        }

        /// <summary>
        /// The Directory for MMTools to use Executables.
        /// </summary>
        public static string MMExecutableDirectory
            => _MMExecutableDirectory ?? throw new MMExecutablesMissingException();

        /// <summary>
        /// Backing field for Executable Directory.
        /// </summary>
        private static string _MMExecutableDirectory;
    }
}
