using MMTools.FFProbeConfig;
using MMTools.Options;
using MMTools.Runners;
using System.Threading.Tasks;

namespace MMTools
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
