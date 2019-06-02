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
            this.AppType = App;
            ApplicationPath = Path.Combine(MMToolsConfiguration.Options.ExecutablesDirectory, App.ToString());
        }

        public virtual async Task Run(string Arguments)
        {
            Console.WriteLine($"Executing {AppType} {Arguments}");
            int result = -1;
            try
            {
                result = await RunProcessAsync(ApplicationPath, Arguments);
            }
            catch (Exception ex)
            {
                throw new MMExecutionException(AppType, ApplicationPath, Arguments, ErrorData, result, ex);
            }

            if (result != 0)
            {
                throw new MMExecutionException(AppType, ApplicationPath, Arguments, ErrorData, result);
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
                if (e.Data == null)
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
                }

                if (LogOutput)
                {
                    Console.WriteLine(e.Data);
                }
                OutputData += "\n" + e.Data;
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (LogError)
                {
                    Console.WriteLine(e.Data);
                }
                ErrorData += "\n" + e.Data;
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return task.Task;
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
        public bool LogOutput { get; set; } = true;
        public bool LogError { get; set; } = true;
        protected MMAppType AppType { get; }
        protected List<MMInputOutputStream> StreamsForPipe { get; private set; } = new List<MMInputOutputStream>();
        private List<NamedPipeServerStream> Pipes = new List<NamedPipeServerStream>();
    }
}