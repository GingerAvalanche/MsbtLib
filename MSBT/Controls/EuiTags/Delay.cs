using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EuiTags
{
    internal class Delay : Control
    {
        public const string Tag = nameof(Delay);
        public const ushort TagType = 0x0000;
        private const ushort ParamSize = 4;
        private readonly uint _frames;
        public Delay(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Delay parameter size mismatch");
            _frames = queue.DequeueU32();
        }
        public Delay(string str)
        {
            Regex pattern = new($@"<{Tag}=(\d+)\s/>");
            Match m = pattern.Match(str);
            bool success = uint.TryParse(m.Groups[1].ToString(), out uint temp);
            if (!success || temp > 65535)
            {
                throw new Exception($"Proper usage: <{Tag}=# /> where # is a number of frames between 0 and 65535. Valid examples: <{Tag}=30 /> or <{Tag}=60 />");
            }
            _frames = temp;
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
