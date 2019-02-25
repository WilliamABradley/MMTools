using System.Collections.Generic;

namespace MMTools.Runners
{
    public class FFMPEGTaskRunner
        : MMTaskRunner
    {
        public FFMPEGTaskRunner(FFMPEGTask Task)
            : base(MMAppType.FFMPEG)
        {
            this.Task = Task;
        }

        public FFMPEGTask Task { get; }

        protected override void AddArgs(ref List<KeyValuePair<string, object>> args)
        {
            foreach (var input in Task.Inputs)
            {
                AddArgNotNull(ref args, "framerate", input.FrameRate);
                AddArgNotNull(ref args, "ss", input.Seek);
                AddArgNotNull(ref args, "t", input.Duration);
                AddArgNotNull(ref args, "vsync", input.VSync?.ToString()?.ToLower());
                AddArgNotNull(ref args, "i", input.Input);
            }

            // Video
            AddArgNotNull(ref args, "vcodec", Task.Options.VideoCodec);
            AddArgNotNull(ref args, "pix_fmt", Task.Options.PixelFormat);
            AddArgNotNull(ref args, "s", Task.Options.Resolution);

            // Audio
            AddArgNotNull(ref args, "acodec", Task.Options.AudioCodec);
            AddArgNotNull(ref args, "ac", Task.Options.AudioChannels);
            AddArgNotNull(ref args, "an", Task.Options.DisableAudioRecording);
            AddArgNotNull(ref args, "ar", Task.Options.AudioSamplingFrequency);

            // Output
            AddArgNotNull(ref args, "shortest", Task.Options.Shortest);
            AddArgNotNull(ref args, "framerate", Task.Output.Duration);
            AddArgNotNull(ref args, "vn", Task.Output.NoVideo);
            AddArgNotNull(ref args, "t", Task.Output.Duration);
            AddArgNotNull(ref args, "#out", Task.Output.Output);
            AddArgNotNull(ref args, "y", Task.Output.Overwrite);

            // Additional options
            AddArgNotNull(ref args, "thread_queue_size", MMToolsConfiguration.Options.ThreadQueueSize);
        }
    }
}
