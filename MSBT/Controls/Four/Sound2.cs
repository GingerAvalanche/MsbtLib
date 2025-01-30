using System.Text.RegularExpressions;

namespace MsbtLib.Controls.Four
{
    internal class Sound2 : Control
    {
        public const ushort TagType = 0x0001;
        private const ushort ParamSize = 2;
        private readonly byte _field1;
        public Sound2(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Sound2 parameter size mismatch");
            _field1 = queue.DequeueU8();
            if (queue.DequeueU8() != 0xCD) throw new InvalidDataException("Sound2 ending byte not 0xCD");
        }
        public Sound2(string str)
        {
            Regex pattern = new(@"<sound2=(-?\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException("Proper usage: <sound2=# /> where # is an 8-bit integer. Valid example: <sound=7 />");
            }
            int temp = int.Parse(m.Groups[1].ToString());
            if (temp < 0 || temp > 255)
            {
                throw new ArgumentException($"Sound2 field is invalid. Must be a number between 0 and 255, was: {temp}");
            }
            _field1 = (byte)temp;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(FourTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes[8] = _field1;
            bytes[9] = 0xCD;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<sound2={_field1} />";
        }
    }
}
