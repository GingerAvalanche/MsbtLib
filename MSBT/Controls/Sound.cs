using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class Sound : Control
    {
        public const ushort tag_group = 0x0003;
        public const ushort tag_type = 0x0001;
        private ushort param_size;
        private byte field_1;
        private byte field_2;
        public Sound(List<ushort> parameters)
        {
            param_size = parameters[0];
            byte[] temp = BitConverter.GetBytes(parameters[1]);
            field_1 = temp[0];
            field_2 = temp[1];
        }
        public Sound(string str)
        {
            Regex pattern = new(@"<sound\sfield_1=(-?\d+)\sfield_2=(-?\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException("Proper usage: <sound field_1=# field_2=# /> where # are both integers. Valid example: <sound field_1=5 field_2=4 />");
            }
            int temp = int.Parse(m.Groups[1].ToString());
            if (temp < 0 || temp > 255)
            {
                throw new ArgumentException($"Sound field_1 is invalid. Must be a number between 0 and 255, was: {temp}");
            }
            field_1 = (byte)temp;
            temp = int.Parse(m.Groups[2].ToString());
            if (temp < 0 || temp > 255)
            {
                throw new ArgumentException($"Sound field_2 is invalid. Must be a number between 0 and 255, was: {temp}");
            }
            field_2 = (byte)temp;
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
            bytes[9] = field_2;
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<sound field_1={field_1} field_2={field_2} />";
        }
    }
}
