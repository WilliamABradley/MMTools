using System.Collections.Generic;

namespace MMTools
{
    public class FFMPEGOutput : FFMPEGOptionsBase
    {
        public IMMInputOutput Output { get; set; }
        public bool Overwrite { get; set; }
        public bool NoVideo { get; set; }

        /// <summary>
        /// A Map of the Streams to use in the Output.
        /// </summary>
        public List<string> Maps { get; set; }

        /// <summary>
        /// The Constant Rate Factor for Quality.
        /// </summary>
        public int? ConstantRateFactor { get; set; }

        /// <summary>
        /// Is the Video Configured for Fast Start.
        /// </summary>
        public bool FastStart { get; set; }

        /// <summary>
        /// Set the number of video frames to output
        /// </summary>
        public int? Frames { get; set; }
    }
}