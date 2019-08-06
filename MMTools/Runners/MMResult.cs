namespace MMTools.Runners
{
    public class MMResult
    {
        internal MMResult()
        {
        }

        public int ResultCode { get; set; }
        public string[] Errors { get; set; }
        public string[] Output { get; set; }

        public string ExtractTrailingJson()
        {
            string normReport = "";
            var index = Errors.Length - 1;
            while (index >= 0)
            {
                var line = Errors[index];
                index--;
                if (line == null)
                {
                    continue;
                }

                normReport = line + normReport;
                if (line.StartsWith("{"))
                {
                    break;
                }
            }

            return normReport;
        }
    }
}