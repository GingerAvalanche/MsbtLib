using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class Animation : Control
    {
        public const ushort tag_group = 0x0004;
        public const ushort tag_type = 0x0002;
        private ushort param_size;
        private ushort _name_size;
        private string _name;
        public Animation(List<ushort> parameters)
        {
            param_size = parameters[0];
            _name_size = parameters[1];
            _name = string.Join("", parameters.GetRange(2, _name_size / 2).Select(u => (char)u));
        }
        public Animation(string str)
        {
            Regex pattern = new(@"<animation=((?:\w|\d)+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException("Proper usage: <animation=? /> where ? is a string with no whitespace. Valid examples: <animation=Activate /> or <animation=CustomLaugh />");
            }
            _name = m.Groups[1].ToString();
            _name_size = (ushort)(_name.Length * 2);
            param_size = (ushort)(_name_size + 2);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes(_name_size), 8);
            bytes.Merge(_name.SelectMany(u => converter.GetBytes(u)), 10);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<animation={_name} />";
        }
    }
}
