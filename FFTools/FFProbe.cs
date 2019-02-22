using FFTools.FFProbeConfig;
using FFTools.Options;
using System.Threading.Tasks;

namespace FFTools
{
    public static class FFProbe
    {
        public static Task<FFProbeInfo> Probe(FFProbeOptions Options)
        {
            var runner = new FFProbeTaskRunner(Options);
            return runner.Run();
        }
    }
}
