using System.Text;

namespace MsbtLib
{
    public class EndiannessConverter(Endianness endianness)
    {
        public Endianness Endianness { get; set; } = endianness;

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
            return Endianness switch
            {
                Endianness.Big => Encoding.BigEndianUnicode.GetBytes(input),
                _ => Encoding.Unicode.GetBytes(input),
            };
        }
        public ushort Convert(ushort input)
        {
            return Endianness switch
            {
                Endianness.Big => Util.ReverseBytes(input),
                _ => input,
            };
        }
        public uint Convert(uint input)
        {
            return Endianness switch
            {
                Endianness.Big => Util.ReverseBytes(input),
                _ => input,
            };
        }
        public ulong Convert(ulong input)
        {
            return Endianness switch
            {
                Endianness.Big => Util.ReverseBytes(input),
                _ => input,
            };
        }
        public static List<byte> ConvertLabelToRaw(List<char> input)
        {
            return Encoding.ASCII.GetBytes(input.ToArray()).ToList();
        }
        public static List<char> ConvertRawToLabel(List<byte> input)
        {
            return Encoding.ASCII.GetString(input.ToArray()).ToList();
        }
    }
}
