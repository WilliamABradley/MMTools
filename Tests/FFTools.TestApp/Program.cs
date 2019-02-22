using FFTools.Runners;
using System.Threading.Tasks;

namespace FFTools.TestApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            FFToolsConfiguration.RegisterExecutableDirectory();

            var ffRunner = new FFRunner(FFAppType.FFMPEG);
            await ffRunner.Run("-h");

            await Task.Delay(50000);
        }
    }
}
