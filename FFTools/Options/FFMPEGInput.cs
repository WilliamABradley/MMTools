using FFTools.Options;

namespace FFTools
{
    public class FFMPEGInput : FFMPEGOptionsBase
    {
        public string Input { get; set; }
        public double? InputOffset { get; set; }
        public VsyncType? VSync { get; set; }
    }
}
