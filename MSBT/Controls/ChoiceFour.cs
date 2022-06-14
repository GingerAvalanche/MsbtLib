using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class ChoiceFour : Control
    {
        public const ushort tag_group = 0x0001;
        public const ushort tag_type = 0x0006;
        private ushort param_size;
        private ushort choice_1;
        private ushort choice_2;
        private ushort choice_3;
        private ushort choice_4;
        private ushort cancel_index;
        public ChoiceFour(List<ushort> parameters)
        {
            param_size = parameters[0];
            choice_1 = parameters[1];
            choice_2 = parameters[2];
            choice_3 = parameters[3];
            choice_4 = parameters[4];
            cancel_index = parameters[5];
        }
        public ChoiceFour(string str)
        {
            Regex pattern = new(@"<choice4\s1=(\d+)\s2=(\d+)\s3=(\d+)\s4=(\d+)\sdefault=(\d+)\scancel=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException(@"Proper usage: <choice4 1=# 2=# 3=# 4=# cancel=# /> where all # are 16-bit 
                    integers, and cancel is the 0-based index of the choice that ends the dialogue. Valid examples: 
                    <choice4 1=34 2=33 3=7 4=8 cancel=3 /> or 
                    <choice4 1=16 2=17 3=18 4=22 cancel=3 />");
            }
            choice_1 = ushort.Parse(m.Groups[1].ToString());
            choice_2 = ushort.Parse(m.Groups[2].ToString());
            choice_3 = ushort.Parse(m.Groups[3].ToString());
            choice_4 = ushort.Parse(m.Groups[4].ToString());
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
            bytes.Merge(converter.GetBytes(choice_1), 8);
            bytes.Merge(converter.GetBytes(choice_2), 10);
            bytes.Merge(converter.GetBytes(choice_3), 12);
            bytes.Merge(converter.GetBytes(choice_4), 14);
            bytes.Merge(converter.GetBytes(cancel_index), 16);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<choice4 1={choice_1} 2={choice_2} 3={choice_3} 4={choice_4} cancel={cancel_index} />";
        }
    }
}
