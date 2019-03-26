using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
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
            ConfigurePipes();
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
                try
                {
                    process?.Dispose();
                }
                catch
                {
                }

                foreach (var pipe in Pipes)
                {
                    try
                    {
                        pipe?.Dispose();
                    }
                    catch
                    {
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return task.Task;
        }

        protected int RunProcess(string program, string args = null)
        {
            try
            {
                ConfigurePipes();

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
            finally
            {
                foreach (var pipe in Pipes)
                {
                    try
                    {
                        pipe?.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void ConfigurePipes()
        {
            // Piping Streams.
            for (int i = 0; i < StreamsForPipe.Count; i++)
            {
                var source = StreamsForPipe[i];
                var direction = source.Input ? PipeDirection.Out : PipeDirection.In;

                var pipeName = $"mm-{i}";
                var pipe = new NamedPipeServerStream(pipeName, direction, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                Pipes.Add(pipe);

                AwaitConnection(pipe, pipeName, direction, source.Stream);
            }
        }

        private void AwaitConnection(NamedPipeServerStream pipe, string pipeName, PipeDirection direction, Stream stream)
        {
            // Wait for Connection.
            pipe.BeginWaitForConnection(asyncResult =>
            {
                // note that the body of the lambda is not part of the outer try... catch block!
                using (var conn = (NamedPipeServerStream)asyncResult.AsyncState)
                {
                    // Connection Connected.
                    try
                    {
                        conn.EndWaitForConnection(asyncResult);
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    Console.WriteLine($"Piping Stream {pipeName}");
                    switch (direction)
                    {
                        case PipeDirection.Out:
                            stream.CopyTo(pipe);
                            // Flush Pipe.
                            pipe.Flush();

                            // do business with the client
                            pipe.WaitForPipeDrain();
                            break;

                        case PipeDirection.In:
                            pipe.CopyTo(stream);
                            break;
                    }
                }

                //AwaitConnection(pipe, pipeName, direction, stream);
            }, pipe);
        }

        protected string ApplicationPath { get; }
        protected string OutputData { get; private set; }
        protected string ErrorData { get; private set; }
        public bool RunSync { get; set; }
        protected List<MMInputOutputStream> StreamsForPipe { get; private set; } = new List<MMInputOutputStream>();
        private List<NamedPipeServerStream> Pipes = new List<NamedPipeServerStream>();
    }
}