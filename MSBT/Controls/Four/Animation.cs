using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.Four
{
    internal class Animation : Control
    {
        public const string Tag = nameof(Animation);
        public const ushort TagType = 0x0002;
        private readonly ushort _paramSize;
        private readonly ushort _nameSize;
        private readonly string _name;
        public Animation(ref VariableByteQueue queue)
        {
            _paramSize = queue.DequeueU16();
            _nameSize = queue.DequeueU16();
            if (_paramSize != _nameSize + 2) throw new InvalidDataException("Animation parameter size mismatch");
            StringBuilder b = new();
            for (int i = 0; i < _nameSize; ++i)
            {
                b.Append((char)queue.DequeueU8());
            }
            _name = b.ToString();
        }
        public Animation(string str)
        {
            Regex pattern = new($@"<{Tag}=((?:\w|\d)+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($"Proper usage: <{Tag}=? /> where ? is a string with no whitespace. Valid examples: <{Tag}=Activate /> or <{Tag}=CustomLaugh />");
            }
            _name = m.Groups[1].ToString();
            _nameSize = (ushort)(_name.Length * 2);
            _paramSize = (ushort)(_nameSize + 2);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[_paramSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(FourTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(_paramSize), 6);
            bytes.Merge(converter.GetBytes(_nameSize), 8);
            bytes.Merge(_name.SelectMany(u => converter.GetBytes(u)), 10);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag}={_name} />";
        }
    }
}
