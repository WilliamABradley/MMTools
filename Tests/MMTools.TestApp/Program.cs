using MMTools.Runners;
using System.Threading.Tasks;

namespace MMTools.TestApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            MMToolsConfiguration.RegisterExecutableDirectory();

            var ffRunner = new MMRunner(MMAppType.FFMPEG);
            await ffRunner.Run("-h");

            await Task.Delay(50000);
        }
    }
}
