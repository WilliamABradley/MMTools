using Newtonsoft.Json;
using System.Collections.Generic;

namespace MMTools.FFProbeConfig.Stream
{
    public class FFStream
    {
        public uint Index { get; set; }

        [JsonProperty("codec_name")]
        public string CodecName { get; set; }

        [JsonProperty("codec_long_name")]
        public string CodecLongName { get; set; }

        [JsonProperty("codec_type")]
        public string CodecType { get; set; }

        [JsonProperty("codec_time_base")]
        public string CodecTimeBase { get; set; }

        [JsonProperty("codec_tag_string")]
        public string CodecTagString { get; set; }

        [JsonProperty("codec_tag")]
        public string CodecTag { get; set; }

        [JsonProperty("r_frame_rate")]
        public string RFrameRate { get; set; }

        [JsonProperty("avg_frame_rate")]
        public string AvgFrameRate { get; set; }

        [JsonProperty("time_base")]
        public string TimeBase { get; set; }

        [JsonProperty("start_pts")]
        public int StartPTS { get; set; }

        [JsonProperty("start_time")]
        public double StartTime { get; set; }

        [JsonProperty("duration_ts")]
        public int DurationTS { get; set; }

        public double Duration { get; set; }

        [JsonProperty("bit_rate")]
        public int Bitrate { get; set; }

        [JsonProperty("nb_frames")]
        public int FrameCount { get; set; }

        public FFDisposition Disposition { get; set; }
        public Dictionary<string, string> Tags { get; set; }

        public override string ToString()
        {
            return $"Stream {Index} ({CodecType}:{CodecLongName})";
        }
    }
}
