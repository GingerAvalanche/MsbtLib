namespace MsbtLib
{
    public class Header(
        byte[] magic,
        EndiannessConverter converter,
        ushort unknown1,
        UtfEncoding encoding,
        byte version,
        ushort sectionCount,
        ushort unknown2,
        uint fileSize,
        byte[] padding)
        : ICalculatesSize
    {
        public readonly byte[] Magic = magic;
        public readonly EndiannessConverter Converter = converter;
        public readonly ushort Unknown1 = unknown1;
        public UtfEncoding Encoding = encoding;
        public readonly byte Version = version;
        public ushort SectionCount = sectionCount;
        public readonly ushort Unknown2 = unknown2;
        public uint FileSize = fileSize;
        public readonly byte[] Padding = padding;

        public ulong CalcSize()
        {
            return sizeof(byte) * 8 // magic
                + sizeof(ushort) // endianness
                + sizeof(ushort) // _unknown_1
                + sizeof(byte) // encoding
                + sizeof(byte) // version
                + sizeof(ushort) // section_count
                + sizeof(ushort) // _unknown_2
                + sizeof(uint) // file_size
                + sizeof(byte) * 10; // padding
        }
    }
}
