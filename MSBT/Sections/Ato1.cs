namespace MsbtLib.Sections
{
    internal class Ato1(SectionHeader section, byte[] bytes) : ICalculatesSize
    {
        public readonly SectionHeader Section = section;
        public readonly byte[] Unknown = bytes;

        public ulong CalcSize() => Section.CalcSize() + (ulong)Unknown.Length;
    }
}
