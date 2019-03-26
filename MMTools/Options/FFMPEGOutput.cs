namespace MMTools
{
    public class FFMPEGOutput : FFMPEGOptionsBase
    {
        public IMMInputOutput Output { get; set; }
        public bool Overwrite { get; set; }
        public bool NoVideo { get; set; }

        /// <summary>
        /// Set the number of video frames to output
        /// </summary>
        public int? Frames { get; set; }
    }
}