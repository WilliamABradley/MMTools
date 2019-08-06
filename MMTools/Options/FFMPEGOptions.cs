using System.Collections.Generic;

namespace MMTools
{
    public class FFMPEGOptions
    {
        public string VideoCodec { get; set; }
        public string PixelFormat { get; set; }

        public bool Shortest { get; set; }

        public string AudioCodec { get; set; }
        public int? AudioChannels { get; set; }
        public bool DisableAudioRecording { get; set; }
        public int? AudioSamplingFrequency { get; set; }

        /// <summary>
        /// A Map of the Streams to use in the Output.
        /// </summary>
        public List<string> Maps { get; set; }
    }
}