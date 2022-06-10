using JetBrains.Annotations;
using System.Text;

namespace MsbtLib.Sections
{
    internal class Txt2 : ICalculatesSize, IUpdates
    {
        [NotNull]
        public Header header;
        public SectionHeader section;
        public uint string_count;
        public List<List<byte>> raw_strings;

        public Txt2(Header header, SectionHeader section, uint string_count, IEnumerable<string> strings)
        {
            this.header = header;
            this.section = section;
            this.string_count = string_count;
            SetStrings(strings);
        }

        public List<string> Strings()
        {
            return header.encoding switch {
                UTFEncoding.UTF16 => raw_strings
                    .Select(s => Util.ToStringUnicode(s))
                    .ToList(),
                _ => raw_strings
                    .Select(r => Util.StripNull(Encoding.UTF8.GetString(r.ToArray())))
                    .ToList(),
            };
        }

        public void SetStrings(IEnumerable<string> strings)
        {
            switch (header.encoding) {
                case UTFEncoding.UTF16:
                    raw_strings = strings
                        .Select(s => Encoding.Unicode.GetBytes(Util.AppendNull(s)).ToList())
                        .ToList();
                    break;
                case UTFEncoding.UTF8:
                    raw_strings = strings
                        .Select(r => Encoding.UTF8.GetBytes(Util.AppendNull(r)).ToList())
                        .ToList();
                    break;
            }
        }

        public ulong CalcSize() => (ulong)((int)section.CalcSize()
            + sizeof(uint) // Marshal.SizeOf(string_count)
            + sizeof(uint) * raw_strings.Count
            + raw_strings.Select(s => s.Count).Sum());

        public void Update()
        {
            section.size = (uint)(sizeof(uint) // Marshal.SizeOf(string_count)
                + sizeof(uint) * raw_strings.Count
                + raw_strings.Select(c => c.Count).Sum());
        }
    }
}
