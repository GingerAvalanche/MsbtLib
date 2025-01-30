using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EUI
{
    internal class AutoAdvance : Control
    {
        public const string Tag = nameof(AutoAdvance);
        public const ushort TagType = 0x0003;
        private const ushort ParamSize = 4;
        private readonly uint _frames;
        public AutoAdvance(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Auto advance parameter size mismatch");
            _frames = queue.DequeueU32();
        }
        public AutoAdvance(string str)
        {
            Regex pattern = new($@"<{Tag}=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success || uint.Parse(m.Groups[1].ToString()) > 65535)
            {
                throw new Exception($"Proper usage: <{Tag}=# /> where # is a number of frames between 0 and 65535. Valid examples: <{Tag}=30 /> or <{Tag}=60 />");
            }
            _frames = uint.Parse(m.Groups[1].ToString());
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(EuiTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes.Merge(converter.GetBytes(_frames), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag}={_frames} />";
        }
    }
}
