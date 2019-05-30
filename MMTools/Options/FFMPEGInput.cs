using MMTools.Options;

namespace MMTools
{
    public class FFMPEGInput : FFMPEGOptionsBase
    {
        /// <summary>
        /// Determines the Start Number for Image Sequence Source.
        /// </summary>
        public int? StartNumber { get; set; }

        public IMMInputOutput Input { get; set; }
        public double? InputOffset { get; set; }
        public VsyncType? VSync { get; set; }
    }
}