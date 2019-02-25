using System;

namespace MMTools.Exceptions
{
    public class MMNotRegisteredException
        : Exception
    {
        internal MMNotRegisteredException()
            : base("MMTools Configuration not registered. Please call MMToolsConfiguration.Register()")
        {
        }
    }
}
