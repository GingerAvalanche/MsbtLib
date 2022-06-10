namespace MsbtLib
{
    public class Header : ICalculatesSize
    {
        public byte[] magic = new byte[8];
        public Endianness endianness;
        public ushort _unknown_1;
        public UTFEncoding encoding;
        public byte version;
        public ushort section_count;
        public ushort _unknown_2;
        public uint file_size;
        public byte[] padding = new byte[10];

        public Header(byte[] magic, Endianness endianness, ushort _unknown_1, UTFEncoding encoding, byte version, ushort section_count, ushort _unknown_2, uint file_size, byte[] padding)
        {
            this.magic = magic;
            this.endianness = endianness;
            this._unknown_1 = _unknown_1;
            this.encoding = encoding;
            this.version = version;
            this.section_count = section_count;
            this._unknown_2 = _unknown_2;
            this.file_size = file_size;
            this.padding = padding;
        }

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
