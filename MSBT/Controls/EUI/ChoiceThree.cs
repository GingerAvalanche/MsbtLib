using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EUI
{
    internal class ChoiceThree : Control
    {
        public const ushort TagType = 0x0005;
        private const ushort ParamSize = 8;
        private readonly ushort _choice0;
        private readonly ushort _choice1;
        private readonly ushort _choice2;
        private readonly byte _defaultIndex;
        private readonly byte _cancelIndex;
        public ChoiceThree(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("ChoiceThree parameter size mismatch");
            _choice0 = queue.DequeueU16();
            _choice1 = queue.DequeueU16();
            _choice2 = queue.DequeueU16();
            _defaultIndex = queue.DequeueU8();
            _cancelIndex = queue.DequeueU8();
        }
        public ChoiceThree(string str)
        {
            Regex pattern = new(@"<choice3\s0=(\d+)\s1=(\d+)\s2=(\d+)\sdefault=(\d)\scancel=(\d)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException(@"Proper usage: <choice3 0=# 1=# 2=# cancel=# /> where all # are 16-bit 
                    integers, and cancel is the 0-based index of the choice that ends the dialogue. Valid examples: 
                    <choice3 0=34 1=33 2=7 default=0 cancel=2 /> or 
                    <choice3 0=16 1=17 2=18 default=1 cancel=2 />");
            }
            _choice0 = ushort.Parse(m.Groups[1].ToString());
            _choice1 = ushort.Parse(m.Groups[2].ToString());
            _choice2 = ushort.Parse(m.Groups[3].ToString());
            _defaultIndex = byte.Parse(m.Groups[4].ToString());
            _cancelIndex = byte.Parse(m.Groups[5].ToString());
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
            bytes.Merge(converter.GetBytes(_choice2), 12);
            bytes[14] = _defaultIndex;
            bytes[15] = _cancelIndex;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<choice3 0={_choice0} 1={_choice1} 2={_choice2} default={_defaultIndex} cancel={_cancelIndex} />";
        }
    }
}
