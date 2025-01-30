using System.Text.RegularExpressions;

namespace MsbtLib.Controls.Three
{
    internal class Sound : Control
    {
        public const string Tag = nameof(Sound);
        public const ushort TagType = 0x0001;
        private const ushort ParamSize = 2;
        private readonly byte _field1;
        private readonly byte _field2;
        public Sound(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Sound parameter size mismatch");
            _field1 = queue.DequeueU8();
            _field2 = queue.DequeueU8();
        }
        public Sound(string str)
        {
            Regex pattern = new(@$"<{Tag}\sfield_1=(-?\d+)\sfield_2=(-?\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($"Proper usage: <{Tag} field_1=# field_2=# /> where # are both integers. Valid example: <{Tag} field_1=5 field_2=4 />");
            }
            int temp = int.Parse(m.Groups[1].ToString());
            if (temp is < 0 or > 255)
            {
                throw new ArgumentException($"{Tag} field_1 is invalid. Must be a number between 0 and 255, was: {temp}");
            }
            _field1 = (byte)temp;
            temp = int.Parse(m.Groups[2].ToString());
            if (temp is < 0 or > 255)
            {
                throw new ArgumentException($"{Tag} field_2 is invalid. Must be a number between 0 and 255, was: {temp}");
            }
            _field2 = (byte)temp;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(ThreeTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes[8] = _field1;
            bytes[9] = _field2;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag} field_1={_field1} field_2={_field2} />";
        }
    }
}
