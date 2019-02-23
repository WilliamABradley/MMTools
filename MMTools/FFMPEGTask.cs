using MMTools.Runners;
using System.Collections.Generic;

namespace MMTools
{
    public class FFMPEGTask
    {
        public FFMPEGTask(FFMPEGOptions Options = null)
        {
            this.Options = Options ?? new FFMPEGOptions();
        }

        public FFMPEGTask AddInput(FFMPEGInput Input)
        {
            Inputs.Add(Input);
            return this;
        }

        public FFMPEGTaskRunner AddOutput(FFMPEGOutput Output)
        {
            this.Output = Output;
            return new FFMPEGTaskRunner(this);
        }

        public FFMPEGOptions Options { get; }

        public List<FFMPEGInput> Inputs { get; } = new List<FFMPEGInput>();
        public FFMPEGOutput Output { get; private set; }
    }
}
