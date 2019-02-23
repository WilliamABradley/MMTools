using Newtonsoft.Json;

namespace MMTools.FFProbeConfig
{
    public class FFDisposition
    {
        public bool Default { get; set; }
        public bool Dub { get; set; }
        public bool Original { get; set; }
        public bool Comment { get; set; }
        public bool Lyrics { get; set; }
        public bool Karaoke { get; set; }
        public bool Forced { get; set; }

        [JsonProperty("hearing_impaired")]
        public bool HairingImpaired { get; set; }

        [JsonProperty("visual_impaired")]
        public bool VisualImpaired { get; set; }

        [JsonProperty("clean_effects")]
        public bool CleanEffects { get; set; }

        [JsonProperty("attached_pic")]
        public bool AttachedPic { get; set; }
    }
}
