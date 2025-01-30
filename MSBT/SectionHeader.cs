namespace MsbtLib
{
    internal class SectionHeader : ICalculatesSize
    {
        public readonly byte[] Magic;
        public uint Size;
        public readonly byte[] Padding = new byte[8];

        public SectionHeader(byte[] magic, uint size)
        {
            Magic = magic;
            Size = size;
        }

        public SectionHeader(byte[] magic, uint size, byte[] padding)
        {
            Magic = magic;
            Size = size;
            Padding = padding;
        }

        public ulong CalcSize() => sizeof(byte) * 12 + sizeof(uint); // Marshal.SizeOf(magic) + Marshal.SizeOf(size) + Marshal.SizeOf(padding)
    }
}
