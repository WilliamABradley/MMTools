using Newtonsoft.Json;
using System.Collections.Generic;

namespace FFTools.FFProbeConfig
{
    public class FFFormat
    {
        public string FileName { get; set; }

        [JsonProperty("nb_streams")]
        public int StreamCount { get; set; }

        [JsonProperty("nb_programs")]
        public int ProgramCount { get; set; }

        [JsonProperty("format_name")]
        public string FormatName { get; set; }

        [JsonProperty("format_long_name")]
        public string FormatLongName { get; set; }

        [JsonProperty("start_time")]
        public double StartTime { get; set; }

        public double Duration { get; set; }

        public int Size { get; set; }

        [JsonProperty("bit_rate")]
        public int Bitrate { get; set; }

        [JsonProperty("probe_score")]
        public int ProbeScore { get; set; }

        public Dictionary<string, string> Tags { get; set; }
    }
}
