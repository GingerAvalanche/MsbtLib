using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class ChoiceThree : Control
    {
        public const ushort tag_group = 0x0001;
        public const ushort tag_type = 0x0005;
        private ushort param_size;
        private ushort choice_0;
        private ushort choice_1;
        private ushort choice_2;
        private ushort cancel_index;
        public ChoiceThree(List<ushort> parameters)
        {
            param_size = parameters[0];
            choice_0 = parameters[1];
            choice_1 = parameters[2];
            choice_2 = parameters[3];
            cancel_index = parameters[4];
        }
        public ChoiceThree(string str)
        {
            Regex pattern = new(@"<choice3\s0=(\d+)\s1=(\d+)\s2=(\d+)\scancel=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException(@"Proper usage: <choice3 0=# 1=# 2=# cancel=# /> where all # are 16-bit 
                    integers, and cancel is the 0-based index of the choice that ends the dialogue. Valid examples: 
                    <choice3 0=34 1=33 2=7 cancel=2 /> or 
                    <choice3 0=16 1=17 2=18 cancel=2 />");
            }
            choice_0 = ushort.Parse(m.Groups[1].ToString());
            choice_1 = ushort.Parse(m.Groups[2].ToString());
            choice_2 = ushort.Parse(m.Groups[3].ToString());
            cancel_index = ushort.Parse(m.Groups[4].ToString());
            param_size = 8;
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
            bytes.Merge(converter.GetBytes(cancel_index), 14);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<choice3 0={choice_0} 1={choice_1} 2={choice_2} cancel={cancel_index} />";
        }
    }
}
