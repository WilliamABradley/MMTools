using System;
using System.IO;

namespace MMTools
{
    public class MMInputOutputStream
        : IMMInputOutput, IDisposable
    {
        public MMInputOutputStream(Stream Stream)
        {
            this.Stream = Stream;
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }

        internal bool Input { get; set; }

        public Stream Stream { get; }
    }
}