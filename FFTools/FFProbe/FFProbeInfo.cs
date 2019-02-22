using FFTools.FFProbeConfig.Stream;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FFTools.FFProbeConfig
{
    public class FFProbeInfo
    {
        [JsonConverter(typeof(FFStreamConverter))]
        public List<FFStream> Streams { get; set; }

        public FFFormat Format { get; set; }
    }
}
