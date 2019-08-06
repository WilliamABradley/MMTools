using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMTools.Runners
{
    public abstract class MMTaskRunner
        : MMRunner
    {
        public MMTaskRunner(MMAppType App) : base(App)
        {
        }

        protected abstract void AddArgs(ref List<KeyValuePair<string, object>> args);

        protected static void AddArgNotNull(ref List<KeyValuePair<string, object>> args, string arg, object val)
        {
            if (val != null)
            {
                args.Add(new KeyValuePair<string, object>(arg, val));
            }
        }

        public override Task<MMResult> Run(string Arguments = null)
        {
            var args = new List<KeyValuePair<string, object>>();
            AddArgs(ref args);

            // Format Arguments
            var argString = string.Join(" ", args.Select(entry =>
            {
                // Output File
                if (entry.Key == "#out")
                {
                    if (entry.Value is IMMInputOutput io)
                    {
                        return GetPath(io);
                    }
                    else
                    {
                        return entry.Value;
                    }
                }
                else if (entry.Key == "#extra")
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
                else if (entry.Value is MMResolution res)
                {
                    return $"-{entry.Key} {res.Width}x{res.Height}";
                }
                else if (entry.Value is IMMInputOutput io)
                {
                    return $"-{entry.Key} {GetPath(io)}";
                }
                else if (entry.Value is MMInputOutputPath path)
                {
                    return $"-{entry.Key} {path.Path}";
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

            return base.Run(argString);
        }

        private string GetPath(IMMInputOutput InputOutput)
        {
            if (InputOutput is MMInputOutputStream stream)
            {
                StreamsForPipe.Add(stream);
                return $"\\\\.\\pipe\\mm-{StreamsForPipe.Count - 1}";
            }
            else if (InputOutput is MMInputOutputPath path)
            {
                return $"\"{path.Path}\"";
            }

            return null;
        }
    }
}