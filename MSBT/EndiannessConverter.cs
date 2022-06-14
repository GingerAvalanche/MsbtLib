using System.Text;

namespace MsbtLib
{
    public class EndiannessConverter
    {
        private Endianness endianness;
        public EndiannessConverter(Endianness endianness)
        {
            this.endianness = endianness;
        }
        public void SetEndianness(Endianness endianness)
        {
            this.endianness = endianness;
        }
        public byte[] GetBytes(ushort input)
        {
            return BitConverter.GetBytes(Convert(input));
        }
        public byte[] GetBytes(uint input)
        {
            return BitConverter.GetBytes(Convert(input));
        }
        public byte[] GetBytes(ulong input)
        {
            return BitConverter.GetBytes(Convert(input));
        }
        public byte[] GetBytes(string input)
        {
            return endianness switch
            {
                Endianness.Big => Encoding.BigEndianUnicode.GetBytes(input),
                _ => Encoding.Unicode.GetBytes(input),
            };
        }
        public ushort Convert(ushort input)
        {
            return endianness switch
            {
                Endianness.Big => Util.ReverseBytes(input),
                _ => input,
            };
        }
        public uint Convert(uint input)
        {
            return endianness switch
            {
                Endianness.Big => Util.ReverseBytes(input),
                _ => input,
            };
        }
        public ulong Convert(ulong input)
        {
            return endianness switch
            {
                Endianness.Big => Util.ReverseBytes(input),
                _ => input,
            };
        }
        static public List<byte> ConvertLabelToRaw(List<char> input)
        {
            return Encoding.ASCII.GetBytes(input.ToArray()).ToList();
        }
        static public List<char> ConvertRawToLabel(List<byte> input)
        {
            return Encoding.ASCII.GetString(input.ToArray()).ToList();
        }
    }
}
