namespace MsbtLib.Sections
{
    internal class Nli1(SectionHeader section, uint idCount, Dictionary<uint, uint> globalIds)
        : ICalculatesSize
    {
        public readonly SectionHeader Section = section;
        public readonly uint IdCount = idCount;
        public readonly Dictionary<uint, uint> GlobalIds = globalIds;

        public ulong CalcSize()
        {
            ulong size = Section.CalcSize();
            if (GlobalIds.Count > 0)
            {
                size += (ulong)(sizeof(uint) // Marshal.SizeOf(id_count)
                    + sizeof(uint) * GlobalIds.Count * 2);
            }
            return size;
        }
    }
}
