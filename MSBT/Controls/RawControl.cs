using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class RawControl : Control
    {
        private ushort tag_group;
        private ushort tag_type;
        private ushort param_size;
        private ushort[] field_1;
        public ushort TagGroup { get => tag_group; set => tag_group = value; }
        public ushort TagType { get => tag_type; set => tag_type = value; }
        public ushort[] Field_1 { get => field_1; set { field_1 = value; param_size = (ushort)(field_1.Length * 2); } }
        public RawControl(ushort tag_group, ushort tag_type, List<ushort> parameters)
        {
            this.tag_group = tag_group;
            this.tag_type = tag_type;
            param_size = parameters[0];
            if (parameters.Count > 1)
            {
                field_1 = parameters.GetRange(1..^0).ToArray();
            }
            else
            {
                field_1 = Array.Empty<ushort>();
            }
        }
        public RawControl(string str)
        {
            Regex pattern = new(@"<raw\sgroup=(\d+)\stype=(\d+)\sdata=""(.*)""\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException(@"Proper usage: <raw group=# type=# data=""?"" /> where # are 16-bit 
                    integers and ? is 0 or more 16-bit integers. Valid examples: <raw group=1 type=8 data=""255 65535"" /> 
                    or <raw group=0 type=4 data="""" />");
            }
            tag_group = ushort.Parse(m.Groups[1].ToString());
            tag_type = ushort.Parse(m.Groups[2].ToString());
            List<int> temp = m.Groups[3].ToString().Split(" ").Select(s => int.Parse(s)).ToList();
            foreach (int i in temp)
            {
                if (i < 0 || i > 65535)
                {
                    throw new ArgumentException($"raw data field is invalid. All numbers must be between 0 and 65535, one was: {i}");
                }
            }
            if (temp.Count > 0)
            {
                field_1 = temp.Select(i => (ushort)i).ToArray();
            }
            else
            {
                field_1 = Array.Empty<ushort>();
            }
            param_size = (ushort)(field_1.Length * 2);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(field_1.SelectMany(u => converter.GetBytes(u)), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<raw group={tag_group} type={tag_type} data=\"{string.Join(" ", field_1)}\" />";
        }
    }
}
