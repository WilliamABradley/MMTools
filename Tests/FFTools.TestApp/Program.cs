using System;
using System.Threading.Tasks;

namespace FFTools.TestApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var ffTask = new FFMPEGTask(new FFMPEGOptions
            {
                Resolution = new FFMPEGResolution(1920, 1080)
            });
            ffTask.AddInput(new FFMPEGInput
            {
                Input = "file1.mp4",
            });
            ffTask.AddOutput(new FFMPEGOutput
            {
                Output = "file2.mp4"
            });

            var runner = new FFMPEGTaskRunner(ffTask);
            await runner.Run();
        }
    }
}
