using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EUI
{
    internal class ChoiceOne : Control
    {
        public const string Tag = "Choice1";
        public const ushort TagType = 0x000A;
        private const ushort ParamSize = 4;
        private readonly ushort _choice;
        public ChoiceOne(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("ChoiceOne parameter size mismatch");
            _choice = queue.DequeueU16();
            if (queue.DequeueU16() != 0x01CD) throw new InvalidDataException("ChoiceOne ending short mismatch");
        }
        public ChoiceOne(string str)
        {
            Regex pattern = new($@"<{Tag}=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($@"Proper usage: <{Tag}=# /> where # is a 16-bit integer. Valid examples: <{Tag}=10 /> or <{Tag}=7 />");
            }
            _choice = ushort.Parse(m.Groups[1].ToString());
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(EuiTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes.Merge(converter.GetBytes(_choice), 8);
            bytes[10] = 0x01;
            bytes[11] = 0xCD;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag}={_choice} />";
        }
    }
}
