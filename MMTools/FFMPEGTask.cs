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
            InputIDCounter++;
            Input.ID = InputIDCounter;
            Inputs.Add(Input);
            return this;
        }

        public FFMPEGTask AddFilter(string Filter)
        {
            Filters.Add(Filter);
            return this;
        }

        public FFMPEGTaskRunner AddOutput(FFMPEGOutput Output)
        {
            this.Output = Output;
            return new FFMPEGTaskRunner(this);
        }

        public FFMPEGOptions Options { get; }

        public List<FFMPEGInput> Inputs { get; } = new List<FFMPEGInput>();
        public List<string> Filters { get; } = new List<string>();
        public FFMPEGOutput Output { get; private set; }
        private int InputIDCounter = -1;
    }
}
