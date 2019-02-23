namespace MMTools
{
    public abstract class FFMPEGOptionsBase
    {
        public double? Seek { get; set; }
        public double? Duration { get; set; }
        public int? FrameRate { get; set; }
    }
}
