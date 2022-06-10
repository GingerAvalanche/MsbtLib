namespace MsbtLib
{
    class SectionHeader : ICalculatesSize
    {
        public byte[] magic = new byte[4];
        public uint size;
        public byte[] padding = new byte[8];

        public SectionHeader(byte[] magic, uint size)
        {
            this.magic = magic;
            this.size = size;
        }

        public SectionHeader(byte[] magic, uint size, byte[] padding)
        {
            this.magic = magic;
            this.size = size;
            this.padding = padding;
        }

        public ulong CalcSize() => sizeof(byte) * 12 + sizeof(uint); // Marshal.SizeOf(magic) + Marshal.SizeOf(size) + Marshal.SizeOf(padding)
    }
}
