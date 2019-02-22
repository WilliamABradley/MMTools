namespace FFTools
{
    public struct FFMPEGResolution
    {
        public FFMPEGResolution(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public int Width { get; }
        public int Height { get; }
    }
}
