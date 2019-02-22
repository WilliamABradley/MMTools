using FFTools.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FFTools
{
    public abstract class FFTaskRunner
    {
        public FFTaskRunner(string ApplicationName)
        {
            ApplicationPath = Path.Combine(FFToolsConfiguration.FFExecutableDirectory, ApplicationName);
            if (!File.Exists(ApplicationPath))
            {
                throw new FFExecutablesMissingException();
            }
        }

        protected abstract void AddArgs(ref List<KeyValuePair<string, object>> args);

        protected static void AddArgNotNull(ref List<KeyValuePair<string, object>> args, string arg, object val)
        {
            if (val != null)
            {
                args.Add(new KeyValuePair<string, object>(arg, val));
            }
        }

        public virtual async Task Run()
        {
            var args = new List<KeyValuePair<string, object>>();
            AddArgs(ref args);

            // Format Arguments
            var argString = string.Join(" ", args.Select(entry =>
            {
                // Output File
                if (entry.Key == "#out")
                {
                    return entry.Value;
                }
                // Flag
                else if (entry.Value is bool state)
                {
                    if (state)
                    {
                        return $"-{entry.Key}";
                    }
                    else
                    {
                        return "";
                    }
                }
                // Resolution
                else if (entry.Value is FFMPEGResolution res)
                {
                    return $"-{entry.Key} {res.Width}x{res.Height}";
                }
                // Not null
                else if (entry.Value != null)
                {
                    return $"-{entry.Key} {entry.Value}";
                }
                // Null values
                else
                {
                    return "";
                }
            }));

            var result = RunSync ? RunProcess(ApplicationPath, argString)
                : await RunProcessAsync(ApplicationPath, argString);
            if (result != 0)
            {
                var appName = Path.GetFileName(ApplicationPath);
                throw new Exception($"An error occurred with {ApplicationPath} ({result}): \n{ErrorData}");
            }
        }

        private Task<int> RunProcessAsync(string program, string args = null)
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

        private int RunProcess(string program, string args = null)
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
        protected bool RunSync { get; set; }
    }
}
