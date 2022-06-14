using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class Sound2 : Control
    {
        public const ushort tag_group = 0x0004;
        public const ushort tag_type = 0x0001;
        private ushort param_size;
        private byte field_1;
        public Sound2(List<ushort> parameters)
        {
            param_size = parameters[0];
            byte[] temp = BitConverter.GetBytes(parameters[1]);
            field_1 = temp[0];
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
            field_1 = (byte)temp;
            param_size = 2;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes[8] = field_1;
            bytes[9] = 0xCD;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<sound2={field_1} />";
        }
    }
}
