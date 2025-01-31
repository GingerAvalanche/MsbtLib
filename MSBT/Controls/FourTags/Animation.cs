using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.FourTags
{
    internal class Animation : Control
    {
        public const string Tag = nameof(Animation);
        public const ushort TagType = 0x0002;
        private readonly ushort _paramSize;
        private readonly string _name;
        public Animation(ref VariableByteQueue queue)
        {
            _paramSize = queue.DequeueU16();
            (ushort nameSize, _name) = queue.DequeueCString();
            if (_paramSize != nameSize + 2) throw new InvalidDataException("Animation parameter size mismatch");
        }
        public Animation(string str)
        {
            Regex pattern = new($@"<{Tag}='((?:\w|\d)+)'\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($"Proper usage: <{Tag}='?' /> where ? is a string with no whitespace. Valid examples: <{Tag}='Activate' /> or <{Tag}='CustomLaugh' />");
            }
            _name = m.Groups[1].ToString();
            _paramSize = (ushort)(_name.Length * 2 + 2);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[_paramSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(FourTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(_paramSize), 6);
            bytes.Merge(converter.GetCString(_name), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag}='{_name}' />";
        }
    }
}
