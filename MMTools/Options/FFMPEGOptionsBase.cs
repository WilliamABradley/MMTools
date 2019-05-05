namespace MMTools
{
    public abstract class FFMPEGOptionsBase
    {
        /// <summary>
        /// Force input or output file format. The format is normally auto detected for input files and guessed from the file extension for output files, so this option is not needed in most cases.
        /// </summary>
        public string Format { get; set; }

        public double? Seek { get; set; }
        public double? Duration { get; set; }
        public int? FrameRate { get; set; }

        /// <summary>
        /// Additional Arguments not specified.
        /// </summary>
        public string AdditionalArgs { get; set; }
    }
}