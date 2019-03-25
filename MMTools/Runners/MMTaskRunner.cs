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

        public override Task Run(string Arguments = null)
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
                else if (entry.Value is MMResolution res)
                {
                    return $"-{entry.Key} {res.Width}x{res.Height}";
                }
                else if (entry.Value is MMInputOutputStream stream)
                {
                    StreamsForPipe.Add(stream);
                    return $"-{entry.Key} \\\\.\\pipe\\mm-{StreamsForPipe.Count - 1}";
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
    }
}