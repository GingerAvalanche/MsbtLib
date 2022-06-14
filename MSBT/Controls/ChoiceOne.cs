using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class ChoiceOne : Control
    {
        public const ushort tag_group = 0x0001;
        public const ushort tag_type = 0x000A;
        private ushort param_size;
        private ushort Choice;
        public ChoiceOne(List<ushort> parameters)
        {
            param_size = parameters[0];
            Choice = parameters[1];
        }
        public ChoiceOne(string str)
        {
            Regex pattern = new(@"<choice1=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException(@"Proper usage: <choice1=# /> where # is a 16-bit integer. Valid examples: <choice1=10 /> or <choice1=7 />");
            }
            Choice = ushort.Parse(m.Groups[1].ToString());
            param_size = 4;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes(Choice), 8);
            bytes[10] = 0x01;
            bytes[11] = 0xCD;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<choice1={Choice} />";
        }
    }
}
