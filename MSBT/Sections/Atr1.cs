using JetBrains.Annotations;

namespace MsbtLib.Sections
{
    internal class Atr1 : ICalculatesSize, IUpdates
    {
        private readonly List<string> _strings;
        [NotNull]
        public Header header;
        public SectionHeader section;
        public uint _unknown_1;
        public List<string> Strings { get => _strings; }

        public Atr1(Header header, SectionHeader section, uint string_count, uint _unknown_1, List<string> strings)
        {
            this.header = header;
            this.section = section;
            this._unknown_1 = _unknown_1;
            _strings = strings;
            if (strings.Count == 0)
            {
                for (int i = 0; i < string_count; i++)
                {
                    strings.Add(string.Empty);
                }
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
                    + _strings.Select(s => Util.StringToRaw(s, header.encoding, header.converter).Count).Sum());
                _unknown_1 = 4;
            }
            else
            {
                _unknown_1 = 0;
            }
            section.size = size;
        }

        public ulong CalcSize()
        {
            ulong size = section.CalcSize() + sizeof(uint) * 2; // Marshal.SizeOf(_unknown_1) + Marshal.SizeOf(_unknown_1)
            if (_strings.Any(s => !string.IsNullOrEmpty(s)))
            {
                size += (ulong)(sizeof(uint) * _strings.Count // offsets
                    + _strings.Select(s => Util.StringToRaw(s, header.encoding, header.converter).Count).Sum());
            }
            return size;
        }
    }
}
