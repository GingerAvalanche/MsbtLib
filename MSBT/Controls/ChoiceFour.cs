using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class ChoiceFour : Control
    {
        public const ushort tag_group = 0x0001;
        public const ushort tag_type = 0x0006;
        private ushort param_size;
        private ushort choice_0;
        private ushort choice_1;
        private ushort choice_2;
        private ushort choice_3;
        private ushort cancel_index;
        public ChoiceFour(List<ushort> parameters)
        {
            param_size = parameters[0];
            choice_0 = parameters[1];
            choice_1 = parameters[2];
            choice_2 = parameters[3];
            choice_3 = parameters[4];
            cancel_index = parameters[5];
        }
        public ChoiceFour(string str)
        {
            Regex pattern = new(@"<choice4\s0=(\d+)\s1=(\d+)\s2=(\d+)\s3=(\d+)\scancel=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException(@"Proper usage: <choice4 0=# 1=# 2=# 3=# cancel=# /> where all # are 16-bit 
                    integers, and cancel is the 0-based index of the choice that ends the dialogue. Valid examples: 
                    <choice4 0=34 1=33 2=7 3=8 cancel=3 /> or 
                    <choice4 0=16 1=17 2=18 3=22 cancel=3 />");
            }
            choice_0 = ushort.Parse(m.Groups[1].ToString());
            choice_1 = ushort.Parse(m.Groups[2].ToString());
            choice_2 = ushort.Parse(m.Groups[3].ToString());
            choice_3 = ushort.Parse(m.Groups[4].ToString());
            cancel_index = ushort.Parse(m.Groups[5].ToString());
            param_size = 10;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes(choice_0), 8);
            bytes.Merge(converter.GetBytes(choice_1), 10);
            bytes.Merge(converter.GetBytes(choice_2), 12);
            bytes.Merge(converter.GetBytes(choice_3), 14);
            bytes.Merge(converter.GetBytes(cancel_index), 16);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<choice4 0={choice_0} 1={choice_1} 2={choice_2} 3={choice_3} cancel={cancel_index} />";
        }
    }
}
