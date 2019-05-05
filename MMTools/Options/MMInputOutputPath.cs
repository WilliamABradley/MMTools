namespace MMTools
{
    public struct MMInputOutputPath
        : IMMInputOutput
    {
        public MMInputOutputPath(string Path)
        {
            this.Path = Path;
        }

        public string Path { get; }
    }
}