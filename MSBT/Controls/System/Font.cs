using System.Text.RegularExpressions;

namespace MsbtLib.Controls.System
{
    enum FontKind
    {
        Hylian = 0x0000,
        Unknown = 0x0004,
        Normal = 0xFFFF
    }
    internal class Font : Control
    {
        public const ushort TagType = 0x0001;
        private const ushort ParamSize = 2;
        private readonly FontKind _face;
        public Font(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Font parameter size mismatch");
            _face = (FontKind)queue.DequeueU16();
        }
        public Font(string str)
        {
            if (str == "</font>")
            {
                _face = FontKind.Normal;
            }
            else
            {
                Regex pattern = new(@"<font=(\w+)>");
                Match m = pattern.Match(str);
                if (!m.Success)
                {
                    throw new ArgumentException("The only recognized font in BOTW is Hylian: <font=Hylian>. Or reset to normal font with </font>");
                }
                _face = (FontKind)Enum.Parse(typeof(FontKind), m.Groups[1].ToString());
            }
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(SystemTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes.Merge(converter.GetBytes((ushort)_face), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            if (_face == FontKind.Normal)
            {
                return "</font>";
            }
            return $"<font={_face}>";
        }
    }
}
