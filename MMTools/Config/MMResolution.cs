namespace MMTools
{
    public struct MMResolution
    {
        public MMResolution(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public int Width { get; }
        public int Height { get; }
    }
}
