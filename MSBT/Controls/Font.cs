using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    enum FontKind
    {
        Hylian = 0x0000,
        Unknown = 0x0004,
        Normal = 0xFFFF
    }
    internal class Font : Control
    {
        public const ushort tag_group = 0x0000;
        public const ushort tag_type = 0x0001;
        private ushort param_size;
        private FontKind Face;
        public Font(List<ushort> parameters)
        {
            param_size = parameters[0];
            Face = (FontKind)parameters[1];
        }
        public Font(string str)
        {
            if (str == "</font>")
            {
                Face = FontKind.Normal;
            }
            else
            {
                Regex pattern = new(@"<font=(\w+)>");
                Match m = pattern.Match(str);
                if (!m.Success)
                {
                    throw new ArgumentException("The only recognized font in BOTW is Hylian: <font=Hylian>. Or reset to normal font with </font>");
                }
                Face = (FontKind)Enum.Parse(typeof(FontKind), m.Groups[1].ToString());
            }
            param_size = 2;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes((ushort)Face), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            if (Face == FontKind.Normal)
            {
                return "</font>";
            }
            return $"<font={Face}>";
        }
    }
}
