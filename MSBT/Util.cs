namespace MsbtLib
{
    class Util
    {
        public static ushort ReverseBytes(ushort val)
        {
            return (ushort)((val & (ushort)0x00FFu) << 8 | (val & (ushort)0xFFu) >> 8);
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

        public static string ToStringUnicode(List<byte> a)
        {
            char control = '\u000E';
            Queue<byte> queue = new(a);
            byte[] charBytes = new byte[2];
            List<char> chars = new();
            while (queue.Count > 0)
            {
                charBytes[0] = queue.Dequeue();
                charBytes[1] = queue.Dequeue();
                char c = BitConverter.ToChar(charBytes);
                if (c == control)
                {
                    chars.AddRange(Control.ParseControlBytes(ref queue).ToControlString());
                }
                else
                {
                    chars.Add(c);
                }
            }
            return StripNull(string.Join("", chars));
        }
    }
}
