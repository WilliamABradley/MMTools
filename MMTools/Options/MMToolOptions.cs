namespace MMTools.Options
{
    public class MMToolOptions
    {
        /// <summary>
        /// Directory containing executables (Executables must be named just ffmpeg, ffprobe, etc. With/Without .exe on Windows).
        /// </summary>
        public string ExecutablesDirectory { get; set; }

        /// <summary>
        /// The Thread Queue Size for FFMPEG.
        /// </summary>
        public int? ThreadQueueSize { get; set; }
    }
}
