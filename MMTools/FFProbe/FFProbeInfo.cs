using MMTools.FFProbeConfig.Stream;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MMTools.FFProbeConfig
{
    public class FFProbeInfo
    {
        [JsonConverter(typeof(FFStreamConverter))]
        public List<FFStream> Streams { get; set; }

        public FFFormat Format { get; set; }
    }
}
