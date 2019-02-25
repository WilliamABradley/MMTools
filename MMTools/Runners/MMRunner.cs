using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MMTools.Runners
{
    public class MMRunner
    {
        public MMRunner(MMAppType App)
        {
            ApplicationPath = Path.Combine(MMToolsConfiguration.Options.ExecutablesDirectory, App.ToString());
        }

        public virtual async Task Run(string Arguments)
        {
            var result = RunSync ? RunProcess(ApplicationPath, Arguments)
                : await RunProcessAsync(ApplicationPath, Arguments);

            if (result != 0)
            {
                var appName = Path.GetFileName(ApplicationPath);
                throw new Exception($"An error occurred with {ApplicationPath} ({result}): \n{ErrorData}");
            }
        }

        protected Task<int> RunProcessAsync(string program, string args = null)
        {
            var task = new TaskCompletionSource<int>();

            // Create the Process
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = program,
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (s, e) =>
            {
                Console.WriteLine(e.Data);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                Console.WriteLine(e.Data);
            };

            process.Exited += (s, e) =>
            {
                task.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return task.Task;
        }

        protected int RunProcess(string program, string args = null)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = program,
                Arguments = args,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });

            process.WaitForExit();
            OutputData = process.StandardOutput.ReadToEnd();
            ErrorData = process.StandardOutput.ReadToEnd();
            return process.ExitCode;
        }

        protected string ApplicationPath { get; }
        protected string OutputData { get; private set; }
        protected string ErrorData { get; private set; }
        public bool RunSync { get; set; }
    }
}
