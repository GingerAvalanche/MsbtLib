using JetBrains.Annotations;

namespace MsbtLib.Sections
{
    internal class Txt2 : ICalculatesSize, IUpdates
    {
        private readonly List<string> _strings;
        [NotNull]
        public Header header;
        public SectionHeader section;
        public List<string> Strings { get => _strings; }

        public Txt2(Header header, SectionHeader section, List<string> strings)
        {
            this.header = header;
            this.section = section;
            _strings = strings;
        }

        public uint AddString(string str)
        {
            uint index = (uint)_strings.Count;
            _strings.Add(str);
            return index;
        }

        public void SetStrings(IEnumerable<string> strings)
        {
            _strings.Clear();
            foreach (string str in strings)
            {
                _strings.Add(str);
            }
        }

        public ulong CalcSize() => (ulong)((int)section.CalcSize()
            + sizeof(uint) // Marshal.SizeOf(string_count)
            + sizeof(uint) * _strings.Count
            + _strings.Select(s => Util.StringToRaw(s, header.encoding, header.converter).Count).Sum());

        public void Update()
        {
            section.size = (uint)(sizeof(uint) // Marshal.SizeOf(string_count)
                + sizeof(uint) * _strings.Count
                + _strings.Select(s => Util.StringToRaw(s, header.encoding, header.converter).Count).Sum());
        }
    }
}
