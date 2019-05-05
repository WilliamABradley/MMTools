using MMTools.Runners;
using System;

namespace MMTools
{
    public class MMExecutionException
        : Exception
    {
        internal MMExecutionException(MMAppType App, string Executable, string Arguments, string Response, int StatusCode, Exception InnerException = null)
            : base($"Execution of {App} Failed ({StatusCode})", InnerException)
        {
            this.App = App;
            this.Executable = Executable;
            this.Arguments = Arguments;
            this.Response = Response;
            this.StatusCode = StatusCode;
        }

        public MMAppType App { get; }
        public string Executable { get; }
        public string Arguments { get; }
        public string Response { get; }
        public int StatusCode { get; }
    }
}