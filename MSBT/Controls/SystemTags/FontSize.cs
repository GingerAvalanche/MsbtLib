using System.Text.RegularExpressions;

namespace MsbtLib.Controls.SystemTags
{
    internal class FontSize : Control
    {
        public const string Tag = nameof(FontSize);
        public const ushort TagType = 0x0002;
        private const ushort ParamSize = 2;
        private readonly ushort _size;
        public FontSize(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("FontSize parameter size mismatch");
            _size = queue.DequeueU16();
        }
        public FontSize(string str)
        {
            Regex pattern = new($@"<{Tag}\spercent=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($"Proper usage: <{Tag} percent=# /> where # is a 16-bit integer. Valid examples: <{Tag} percent=125 /> or <{Tag} percent=100 />");
            }
            int temp = int.Parse(m.Groups[1].ToString());
            if (temp < 0 || temp > 65535)
            {
                throw new ArgumentException($"{Tag} percent is invalid. Must be a number between 0 and 65535, was: {temp}");
            }
            _size = (ushort)temp;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(SystemTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes.Merge(converter.GetBytes(_size), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag} percent={_size} />";
        }
    }
}
