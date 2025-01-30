namespace MsbtLib.Sections
{
    internal class Atr1 : ICalculatesSize, IUpdates
    {
        private readonly List<string> _strings;
        public readonly Header Header;
        public readonly SectionHeader Section;
        public uint Unknown1;
        public List<string> Strings => _strings;

        public Atr1(Header header, SectionHeader section, uint stringCount, uint unknown1, List<string> strings)
        {
            Header = header;
            Section = section;
            Unknown1 = unknown1;
            _strings = strings;
            long toAdd = stringCount - strings.Count;
            for (int i = 0; i < toAdd; i++)
            {
                strings.Add(string.Empty);
            }
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

        public void Update()
        {
            uint size = sizeof(uint) * 2; // Marshal.SizeOf(_unknown_1) + Marshal.SizeOf(_unknown_1)
            if (_strings.Any(s => !string.IsNullOrEmpty(s)))
            {
                size += (uint)(sizeof(uint) * _strings.Count // offsets
                    + _strings.Select(s => Util.StringToRaw(s, Header.Encoding, Header.Converter).Count).Sum());
                Unknown1 = 4;
            }
            else
            {
                Unknown1 = 0;
            }
            Section.Size = size;
        }

        public ulong CalcSize()
        {
            ulong size = Section.CalcSize() + sizeof(uint) * 2; // Marshal.SizeOf(_unknown_1) + Marshal.SizeOf(_unknown_1)
            if (_strings.Any(s => !string.IsNullOrEmpty(s)))
            {
                size += (ulong)(sizeof(uint) * _strings.Count // offsets
                    + _strings.Select(s => Util.StringToRaw(s, Header.Encoding, Header.Converter).Count).Sum());
            }
            return size;
        }
    }
}
