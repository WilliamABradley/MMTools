using Newtonsoft.Json;

namespace MMTools.FFProbeConfig.Stream
{
    public class FFVideoStream : FFStream
    {
        public string Profile { get; set; }

        public int Width { get; set; }
        public int Height { get; }

        [JsonProperty("has_b_frames")]
        public bool HasBFrames { get; set; }

        [JsonProperty("sample_aspect_ratio")]
        public string SampleAspectRatio { get; set; }

        [JsonProperty("display_aspect_ratio")]
        public string DisplayAspectRatio { get; set; }

        [JsonProperty("pix_fmt")]
        public string PixelFormat { get; set; }

        public int Level { get; set; }
    }
}
