using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFTools.Runners
{
    public abstract class FFTaskRunner
        : FFRunner
    {
        public FFTaskRunner(FFAppType App) : base(App)
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

            return base.Run(argString);
        }
    }
}
