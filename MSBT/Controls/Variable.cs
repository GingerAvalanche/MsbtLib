using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class Variable : Control
    {
        public const ushort tag_group = 0x0002;
        public ushort tag_type;
        private ushort param_size;
        private ushort name_length;
        private string name;
        public Variable(ushort tag_type, List<ushort> parameters)
        {
            this.tag_type = tag_type;
            param_size = parameters[0];
            name_length = parameters[1];
            name = Util.StripNull(Encoding.Unicode.GetString(parameters.GetRange(2..^0).SelectMany(u => BitConverter.GetBytes(u)).ToArray()));
        }
        public Variable(string str)
        {
            Match m = new Regex(@"<variable\skind=(\d+)\sname=((?:\w|\d)+)\s/>").Match(str);
            if (!m.Success)
            {
                throw new ArgumentException("Proper usage: <variable kind=# name=? /> where # is a 16-bit integer and ? is a string of letters/numbers only. Valid example: <variable kind=14 name=GiveItemNumber />");
            }
            tag_type = ushort.Parse(m.Groups[1].ToString());
            name = m.Groups[2].ToString();
            name_length = (ushort)(name.Length * 2);
            param_size = (ushort)(name_length + 4);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes(name_length), 8);
            bytes.Merge(Encoding.Unicode.GetBytes(Util.AppendNull(name)), 10);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<variable kind={tag_type} name={name} />";
        }
    }
}
