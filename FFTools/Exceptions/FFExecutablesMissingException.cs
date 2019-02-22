using System;

namespace FFTools.Exceptions
{
    public class FFExecutablesMissingException
        : Exception
    {
        internal FFExecutablesMissingException()
            : base("The Executables folder provided for FFTools is missing. " +
                  "Did you download the Executables NuGet or provide your own FFMPEG? " +
                  "(Use FFToolsConfiguration.RegisterExecutableDirectory)")
        {
        }
    }
}
