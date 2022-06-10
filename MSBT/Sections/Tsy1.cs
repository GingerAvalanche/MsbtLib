using JetBrains.Annotations;

namespace MsbtLib.Sections
{
    internal class Tsy1 : ICalculatesSize
    {
        [NotNull]
        public SectionHeader section;
        public byte[] _unknown;

        public Tsy1(SectionHeader section, byte[] unknown_bytes)
        {
            this.section = section;
            _unknown = unknown_bytes;
        }

        public ulong CalcSize() => section.CalcSize() + (ulong)_unknown.Length;
    }
}
