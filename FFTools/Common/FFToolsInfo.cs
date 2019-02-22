using System;
using System.Runtime.InteropServices;

namespace FFTools.Common
{
    public static class FFToolsInfo
    {
        /// <summary>
        /// Gets the Directory for FFMPEG executables for the Platform.
        /// </summary>
        /// <returns></returns>
        public static string FFToolsDirectory()
        {
            string OSPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OSPath += "windows-";
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
                throw new NotImplementedException("Unknown OS for FFMPEG");
            }

            OSPath += RuntimeInformation.OSArchitecture.ToString().ToLower();
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", OSPath, "FFMPEG");
        }
    }
}
