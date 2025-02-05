using System.Text.RegularExpressions;

namespace MsbtLib.Controls.SystemTags
{
    enum Color
    {
        Red,
        Green,
        Cyan,
        Grey,
        Azure,
        Orange,
        Gold,
        Reset = 0xFFFF
    }
    internal class FontColor : Control
    {
        public const string Tag = "Color";
        public const ushort TagType = 0x0003;
        private const ushort ParamSize = 2;
        private readonly Color _color;
        public FontColor(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("FontColor parameter size mismatch");
            _color = (Color)queue.DequeueU16();
        }
        public FontColor(string str)
        {
            if (str == $"</{Tag}>")
            {
                _color = Color.Reset;
            }
            else
            {
                Match m = new Regex($@"<{Tag}=(\w+)>").Match(str);
                if (!m.Success)
                {
                    throw new Exception($"Proper usage: <{Tag}=?> where ? is one of: Red, Green, Cyan, Grey, Azure, Orange, Gold.");
                }
                _color = (Color)Enum.Parse(typeof(Color), m.Groups[1].ToString());
            }
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(SystemTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes.Merge(converter.GetBytes((ushort)_color), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            if (_color == Color.Reset)
            {
                return $"</{Tag}>";
            }
            return $"<{Tag}={_color}>";
        }
    }
}
