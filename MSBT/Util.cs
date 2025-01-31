using System.Text;

namespace MsbtLib
{
    internal static class Util
    {
        public static ushort ReverseBytes(ushort val)
        {
            return (ushort)((val & (ushort)0x00FFu) << 8 | (val & (ushort)0xFF00u) >> 8);
        }

        public static uint ReverseBytes(uint val)
        {
            return (val & 0x000000FFu) << 24 | (val & 0x0000FF00u) << 8 |
                (val & 0x00FF0000u) >> 8 | (val & 0xFF000000u) >> 24;
        }

        public static ulong ReverseBytes(ulong val)
        {
            return (val & 0x00000000000000FFul) << 56 | (val & 0x000000000000FF00ul) << 40 |
                (val & 0x0000000000FF0000ul) << 24 | (val & 0x00000000FF000000ul) << 8 |
                (val & 0x000000FF00000000ul) >> 8 | (val & 0x0000FF0000000000ul) >> 24 |
                (val & 0x00FF000000000000ul) >> 40 | (val & 0xFF00000000000000ul) >> 56;
        }

        public static string StripNull(string s)
        {
            return s.TrimEnd('\0');
        }

        public static string AppendNull(string s)
        {
            return s + char.MinValue;
        }
        public static List<byte> StringToRaw(string input, UtfEncoding encoding, EndiannessConverter converter)
        {
            Queue<char> queue = new(AppendNull(input));
            List<byte> bytes = new(input.Length);
            List<char> sequence = new(input.Length);
            while (queue.Count > 0)
            {
                char c = queue.Dequeue();
                if (c == '<')
                {
                    char lastChar = c;
                    sequence.Clear();
                    sequence.Add(lastChar);
                    while (lastChar != '>')
                    {
                        lastChar = queue.Dequeue();
                        sequence.Add(lastChar);
                    }
                    bytes.AddRange(Control.GetControl(string.Join("", sequence)).ToControlSequence(converter));
                }
                else
                {
                    switch (encoding)
                    {
                        case UtfEncoding.Utf16:
                            bytes.AddRange(converter.GetBytes(c));
                            break;
                        case UtfEncoding.Utf8:
                            bytes.Add(Convert.ToByte(c));
                            break;
                    }
                }
            }
            return bytes;
        }
        public static string RawToString(ReadOnlySpan<byte> input, UtfEncoding encoding, EndiannessConverter converter)
        {
            char control = '\u000E';
            VariableByteQueue queue = new(ref input, converter.Endianness);
            StringBuilder str = new();
            while (queue.Count > 0)
            {
                char c = encoding switch
                {
                    UtfEncoding.Utf16 => Convert.ToChar(queue.DequeueU16()),
                    _ => Convert.ToChar(queue.DequeueU8()),
                };
                if (c == control)
                {
                    str.Append(Control.GetControl(ref queue).ToControlString());
                }
                else
                {
                    str.Append(c);
                }
            }
            return StripNull(str.ToString());
        }
    }
}
