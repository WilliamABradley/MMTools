using System.Collections.Generic;
using System.Linq;

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
            // Options
            AddArgNotNull(ref args, "thread_queue_size", MMToolsConfiguration.Options.ThreadQueueSize);

            foreach (var input in Task.Inputs)
            {
                AddArgNotNull(ref args, "framerate", input.FrameRate);
                AddArgNotNull(ref args, "itsoffset", input.InputOffset);
                AddArgNotNull(ref args, "ss", input.Seek);
                AddArgNotNull(ref args, "t", input.Duration);
                AddArgNotNull(ref args, "vsync", input.VSync?.ToString()?.ToLower());
                AddArgNotNull(ref args, "f", input.Format);
                AddArgNotNull(ref args, "start_number", input.StartNumber);
                AddArgNotNull(ref args, "s", input.Resolution);
                AddArgNotNull(ref args, "#extra", input.AdditionalArgs);

                // Needs to be last input argument.
                AddArgNotNull(ref args, "i", input.Input);

                // Ensure Marked as Input Stream.
                if (input.Input is MMInputOutputStream stream)
                {
                    stream.Input = true;
                }
            }

            // Video
            AddArgNotNull(ref args, "vcodec", Task.Options.VideoCodec);
            AddArgNotNull(ref args, "pix_fmt", Task.Options.PixelFormat);

            // Audio
            AddArgNotNull(ref args, "acodec", Task.Options.AudioCodec);
            AddArgNotNull(ref args, "ac", Task.Options.AudioChannels);
            AddArgNotNull(ref args, "an", Task.Options.DisableAudioRecording);
            AddArgNotNull(ref args, "ar", Task.Options.AudioSamplingFrequency);

            // Filters
            if (Task.Filters.Any())
            {
                AddArgNotNull(ref args, "filter_complex", "\"" + string.Join(";", Task.Filters) + "\"");
            }

            // Maps
            if (Task.Output.Maps != null)
            {
                foreach (var map in Task.Output.Maps)
                {
                    AddArgNotNull(ref args, "map", map);
                }
            }

            // Output
            AddArgNotNull(ref args, "shortest", Task.Options.Shortest);
            AddArgNotNull(ref args, "framerate", Task.Output.FrameRate);
            AddArgNotNull(ref args, "frames:v", Task.Output.Frames);
            AddArgNotNull(ref args, "vn", Task.Output.NoVideo);
            AddArgNotNull(ref args, "t", Task.Output.Duration);
            AddArgNotNull(ref args, "f", Task.Output.Format);
            AddArgNotNull(ref args, "crf", Task.Output.ConstantRateFactor);
            AddArgNotNull(ref args, "s", Task.Output.Resolution);
            AddArgNotNull(ref args, "#extra", Task.Output.AdditionalArgs);

            // Output file.
            AddArgNotNull(ref args, "#out", Task.Output.Output);
            AddArgNotNull(ref args, "y", Task.Output.Overwrite);
        }
    }
}