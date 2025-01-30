namespace MsbtLib.Sections
{
    internal class Tsy1(SectionHeader section, byte[] unknownBytes) : ICalculatesSize
    {
        public readonly SectionHeader Section = section;
        public readonly byte[] Unknown = unknownBytes;

        public ulong CalcSize() => Section.CalcSize() + (ulong)Unknown.Length;
    }
}
