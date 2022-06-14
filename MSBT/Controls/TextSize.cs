using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class TextSize : Control
    {
        public const ushort tag_group = 0x0000;
        public const ushort tag_type = 0x0002;
        private ushort param_size;
        private ushort Size;
        public TextSize(List<ushort> parameters)
        {
            param_size = parameters[0];
            Size = parameters[1];
        }
        public TextSize(string str)
        {
            Regex pattern = new(@"<textsize\spercent=(-?\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException("Proper usage: <textsize percent=# /> where # is a 16-bit integer. Valid examples: <textsize percent=125 /> or <textsize percent=100 />");
            }
            int temp = int.Parse(m.Groups[1].ToString());
            if (temp < 0 || temp > 65535)
            {
                throw new ArgumentException($"Textsize percent is invalid. Must be a number between 0 and 65535, was: {temp}");
            }
            Size = (ushort)temp;
            param_size = 2;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes(Size), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<textsize percent={Size} />";
        }
    }
}
