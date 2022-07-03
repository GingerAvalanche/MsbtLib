namespace MsbtLib.Sections
{
    internal class Nli1 : ICalculatesSize
    {
        public SectionHeader section;
        public uint id_count;
        public Dictionary<uint, uint> global_ids;

        public Nli1(SectionHeader section, uint id_count, Dictionary<uint, uint> global_ids)
        {
            this.section = section;
            this.id_count = id_count;
            this.global_ids = global_ids;
        }

        public ulong CalcSize()
        {
            ulong size = section.CalcSize();
            if (global_ids.Count > 0)
            {
                size += (ulong)(sizeof(uint) // Marshal.SizeOf(id_count)
                    + sizeof(uint) * global_ids.Count * 2);
            }
            return size;
        }
    }
}
