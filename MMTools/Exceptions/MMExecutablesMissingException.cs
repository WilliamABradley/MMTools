using System;

namespace MMTools.Exceptions
{
    public class MMExecutablesMissingException
        : Exception
    {
        internal MMExecutablesMissingException()
            : base("The Executables folder provided for MMTools is missing. " +
                  "Did you download the Executables NuGet or provide your own executables? " +
                  "(Use MMToolsConfiguration.RegisterExecutableDirectory)")
        {
        }
    }
}
