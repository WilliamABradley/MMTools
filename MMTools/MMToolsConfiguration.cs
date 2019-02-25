using MMTools.Exceptions;
using MMTools.Options;
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
        /// Registers the MMTools Configuration.
        /// </summary>
        /// <param name="Options">Options for MMTools</param>
        public static void Register(MMToolOptions Options = null)
        {
            Options = new MMToolOptions();

            if (string.IsNullOrWhiteSpace(Options.ExecutablesDirectory))
            {
                Options.ExecutablesDirectory = DefaultMMToolsDirectory();
            }

            if (!Directory.Exists(Options.ExecutablesDirectory))
            {
                throw new MMExecutablesMissingException();
            }

            _Options = Options;
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
        /// MMTools Configuration Options. Must call <see cref="Register(MMToolOptions)"/> to be usable.
        /// </summary>
        public static MMToolOptions Options
            => _Options ?? throw new MMNotRegisteredException();

        /// <summary>
        /// Backing Field for MMTools Options.
        /// </summary>
        private static MMToolOptions _Options;
    }
}
