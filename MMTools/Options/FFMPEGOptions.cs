namespace MMTools
{
    public class FFMPEGOptions
    {
        public MMResolution? Resolution { get; set; }
        public string VideoCodec { get; set; }
        public string PixelFormat { get; set; }

        public bool Shortest { get; set; }

        public string AudioCodec { get; set; }
        public int? AudioChannels { get; set; }
        public bool DisableAudioRecording { get; set; }
        public int? AudioSamplingFrequency { get; set; }
    }
}