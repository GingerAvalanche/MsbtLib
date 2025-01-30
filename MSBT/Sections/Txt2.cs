namespace MsbtLib.Sections
{
    internal class Txt2(Header header, SectionHeader section, List<string> strings) : ICalculatesSize, IUpdates
    {
        public readonly Header Header = header;
        public readonly SectionHeader Section = section;
        public List<string> Strings => strings;

        public uint AddString(string str)
        {
            uint index = (uint)strings.Count;
            strings.Add(str);
            return index;
        }

        public void SetStrings(IEnumerable<string> strings1)
        {
            strings.Clear();
            foreach (string str in strings1)
            {
                strings.Add(str);
            }
        }

        public ulong CalcSize() => (ulong)((int)Section.CalcSize()
            + sizeof(uint) // Marshal.SizeOf(string_count)
            + sizeof(uint) * strings.Count
            + strings.Select(s => Util.StringToRaw(s, Header.Encoding, Header.Converter).Count).Sum());

        public void Update()
        {
            Section.Size = (uint)(sizeof(uint) // Marshal.SizeOf(string_count)
                + sizeof(uint) * strings.Count
                + strings.Select(s => Util.StringToRaw(s, Header.Encoding, Header.Converter).Count).Sum());
        }
    }
}
