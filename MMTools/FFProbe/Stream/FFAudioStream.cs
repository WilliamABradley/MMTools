using Newtonsoft.Json;

namespace MMTools.FFProbeConfig.Stream
{
    public class FFAudioStream : FFStream
    {
        [JsonProperty("sample_fmt")]
        public string SampleFormat { get; set; }

        [JsonProperty("sample_rate")]
        public int SampleRate { get; set; }

        public int Channels { get; set; }

        [JsonProperty("channel_layout")]
        public string ChannelLayout { get; set; }

        [JsonProperty("bits_per_sample")]
        public int BitsPerSample { get; set; }
    }
}
