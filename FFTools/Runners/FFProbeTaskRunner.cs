using FFTools.FFProbeConfig;
using FFTools.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFTools
{
    public class FFProbeTaskRunner : FFTaskRunner
    {
        public FFProbeTaskRunner(FFProbeOptions Options)
            : base("ffprobe")
        {
            this.Options = Options;
            RunSync = true;
        }

        protected override void AddArgs(ref List<KeyValuePair<string, object>> args)
        {
            AddArgNotNull(ref args, "i", Options.Input);
            AddArgNotNull(ref args, "print_format", "json");
            AddArgNotNull(ref args, "show_format", true);
            AddArgNotNull(ref args, "show_streams", true);
        }

        public new async Task<FFProbeInfo> Run()
        {
            await base.Run();
            return JsonConvert.DeserializeObject<FFProbeInfo>(OutputData);
        }

        public FFProbeOptions Options { get; }
    }
}
