using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    enum FontColor
    {
        Red,
        LightGreen1,
        Blue,
        Grey,
        LightGreen4,
        Orange,
        LightGrey,
        Reset = 0xFFFF
    }
    internal class SetColor : Control
    {
        public const ushort tag_group = 0x0000;
        public const ushort tag_type = 0x0003;
        private ushort param_size;
        private FontColor Color;
        public SetColor(List<ushort> parameters)
        {
            param_size = parameters[0];
            Color = (FontColor)parameters[1];
        }
        public SetColor(string str)
        {
            if (str == "</color>")
            {
                Color = FontColor.Reset;
            }
            else
            {
                Match m = new Regex(@"<color=(.+)>").Match(str);
                if (!m.Success)
                {
                    throw new Exception("Proper usage: <color=?> where ? is a valid color name. Valid examples: <color=Red> or <color=LightGreen4>");
                }
                Color = (FontColor)Enum.Parse(typeof(FontColor), m.Groups[1].ToString());
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
            bytes.Merge(converter.GetBytes((ushort)Color), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            if (Color == FontColor.Reset)
            {
                return "</color>";
            }
            return $"<color={Color}>";
        }
    }
}
