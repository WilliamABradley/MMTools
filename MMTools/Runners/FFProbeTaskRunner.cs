using MMTools.FFProbeConfig;
using MMTools.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMTools.Runners
{
    public class FFProbeTaskRunner : MMTaskRunner
    {
        public FFProbeTaskRunner(FFProbeOptions Options)
            : base(MMAppType.FFProbe)
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

        public new async Task<FFProbeInfo> Run(string Arguments = null)
        {
            await base.Run(Arguments);
            return JsonConvert.DeserializeObject<FFProbeInfo>(OutputData);
        }

        public FFProbeOptions Options { get; }
    }
}
