using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EuiTags
{
    internal class ChoiceTwo : Control
    {
        public const string Tag = "Choice2";
        public const ushort TagType = 0x0004;
        private const ushort ParamSize = 6;
        private readonly ushort _choice0;
        private readonly ushort _choice1;
        private readonly byte _defaultIndex;
        private readonly byte _cancelIndex;
        public ChoiceTwo(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("ChoiceTwo parameter size mismatch");
            _choice0 = queue.DequeueU16();
            _choice1 = queue.DequeueU16();
            _defaultIndex = queue.DequeueU8();
            _cancelIndex = queue.DequeueU8();
        }
        public ChoiceTwo(string str)
        {
            Regex pattern = new($@"<{Tag}\s0=(\d+)\s1=(\d+)\sdefault=(\d)\scancel=(\d)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($@"Proper usage: <{Tag} 0=# 1=# cancel=# /> where all # are 16-bit 
                    integers, and cancel is the 0-based index of the choice that ends the dialogue. Valid examples: 
                    <{Tag} 0=0 1=1 default=0 cancel=1 /> or 
                    <{Tag} 0=16 1=17 default=1 cancel=1 />");
            }
            _choice0 = ushort.Parse(m.Groups[1].ToString());
            _choice1 = ushort.Parse(m.Groups[2].ToString());
            _defaultIndex = byte.Parse(m.Groups[3].ToString());
            _cancelIndex = byte.Parse(m.Groups[4].ToString());
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(EuiTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes.Merge(converter.GetBytes(_choice0), 8);
            bytes.Merge(converter.GetBytes(_choice1), 10);
            bytes[12] = _defaultIndex;
            bytes[13] = _cancelIndex;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag} 0={_choice0} 1={_choice1} default={_defaultIndex} cancel={_cancelIndex} />";
        }
    }
}
