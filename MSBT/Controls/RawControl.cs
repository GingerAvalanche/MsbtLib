using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class RawControl : Control
    {
        public const string Tag = "Raw";
        private readonly ushort _tagGroup;
        private readonly ushort _tagType;
        private readonly ushort _paramSize;
        private readonly byte[] _field1;

        public RawControl(ushort tagGroup, ref VariableByteQueue queue)
        {
            _tagGroup = tagGroup;
            _tagType = queue.DequeueU16();
            _paramSize = queue.DequeueU16();
            _field1 = new byte[_paramSize];
            for (int i = 0; i < _paramSize; ++i)
            {
                _field1[i] = queue.DequeueU8();
            }
        }
        public RawControl(string str)
        {
            Regex pattern = new(@$"<{Tag}\sgroup=(\d+)\stype=(\d+)\sdata='(.*)'\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($@"Proper usage: <{Tag} group=# type=# data='?' /> where # are 16-bit 
                    integers and ? is 0 or more 8 bit integers. Valid examples: <{Tag} group=1 type=8 data=""0 255 127"" /> 
                    or <{Tag} group=0 type=4 data="""" />");
            }
            _tagGroup = ushort.Parse(m.Groups[1].ToString());
            _tagType = ushort.Parse(m.Groups[2].ToString());
            string tempStr = m.Groups[3].ToString();
            if (!string.IsNullOrEmpty(tempStr))
            {
                List<int> temp = tempStr.Split(" ").Select(int.Parse).ToList();
                foreach (int i in temp)
                {
                    if (i is < 0 or > 255)
                    {
                        throw new ArgumentException($"{Tag} data field is invalid. All numbers must be between 0 and 65535, one was: {i}");
                    }
                }
                _field1 = temp.Select(i => (byte)i).ToArray();
            }
            else
            {
                _field1 = [];
            }
            _paramSize = (ushort)(_field1.Length * 2);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[_paramSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(_tagGroup), 2);
            bytes.Merge(converter.GetBytes(_tagType), 4);
            bytes.Merge(converter.GetBytes(_paramSize), 6);
            bytes.Merge(_field1, 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<{Tag} group={_tagGroup} type={_tagType} data='{string.Join(" ", _field1)}' />";
        }
    }
}
