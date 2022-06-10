using JetBrains.Annotations;
using System.Text;

namespace MsbtLib.Sections
{
    internal class Lbl1 : ICalculatesSize, IUpdates
    {
        [NotNull]
        public MSBT msbt;
        public SectionHeader section;
        public uint group_count;
        public List<Group> groups;
        public List<Label> labels;

        public Lbl1() { }
        public Lbl1(MSBT msbt, SectionHeader section, uint group_count, List<Group> groups, List<Label> labels)
        {
            this.msbt = msbt;
            this.section = section;
            this.group_count = group_count;
            this.groups = groups;
            this.labels = labels;
        }

        public void UpdateGroups()
        {
            int total = 0;
            int group_len = groups.Count;
            foreach (var (group, i) in groups.Select((group, i) => (group, i))) {
                group.offset = (uint)(group_len * (int)group.CalcSize()
                    + sizeof(uint) // Marshal.SizeOf(group_count)
                    + total);
                group.label_count = (uint)labels.Where(x => x.checksum == i).Count();
                total = labels
                    .Where(x => x.checksum == i)
                    .Aggregate(0, (current, lbl) => current + (int)lbl.CalcSize());
            }
        }

        public ulong CalcSize() => section.CalcSize()
            + (ulong)(sizeof(uint) // Marshal.SizeOf(group_count)
            + groups.Select(g => (int)g.CalcSize()).Sum()
            + labels.Select(l => (int)l.CalcSize()).Sum());

        public void Update()
        {
            section.size = (uint)(CalcSize() - section.CalcSize());
            labels.Sort((l1, l2) => l1.index.CompareTo(l2.index));
            labels.Sort((l1, l2) => l1.checksum.CompareTo(l2.checksum));
            UpdateGroups();
            if (msbt.txt2 != null) {
                msbt.txt2.Update();
            }
        }
    }
    class Group : ICalculatesSize
    {
        public uint label_count;
        public uint offset;

        public Group(uint label_count, uint offset)
        {
            this.label_count = label_count;
            this.offset = offset;
        }

        public ulong CalcSize() => sizeof(uint) * 2; // Marshal.SizeOf(label_count) + Marshal.SizeOf(offset)
    }
    internal class Label : ICalculatesSize
    {
        static readonly uint HASH_MAGIC = 0x492;
        private string p_name;

        [NotNull]
        public Lbl1 lbl1;
        public string name { get => p_name; set { p_name = value; checksum = GenerateChecksum(value, lbl1.group_count); } }
        public uint index;
        public uint checksum { get; private set; }


        public Label() { lbl1 = new(); p_name = ""; }
        public Label(Lbl1 lbl1, string name, uint index, uint checksum)
        {
            this.lbl1 = lbl1;
            p_name = name;
            this.index = index;
            this.checksum = checksum;
        }

        public static uint GenerateChecksum(string name, uint group_count)
        {
            unchecked {
                return Encoding.UTF8.GetBytes(name).ToList().Aggregate(0u, (hash, b) => hash * HASH_MAGIC + b) % group_count;
            }
        }

        public string Value => lbl1.msbt.txt2.Strings()[(int)index];
        public string ValueUnchecked => Value;
        public byte[] ValueRaw => lbl1.msbt.txt2.raw_strings[(int)index].ToArray();
        public byte[] ValueRawUnchecked => ValueRaw;

        public void SetValue(string s)
        {
            switch (lbl1.msbt.header.encoding) {
                case UTFEncoding.UTF16:
                    SetValueRaw(Encoding.Unicode.GetBytes(s));
                    break;
                case UTFEncoding.UTF8:
                    SetValueRaw(Encoding.UTF8.GetBytes(s));
                    break;
            }
        }

        public bool SetValueRaw(byte[] value)
        {
            lbl1.msbt.txt2.raw_strings[(int)index] = value.ToList();
            return true;
        }

        public ulong CalcSize() => (ulong)(sizeof(byte) // name length
            + Encoding.UTF8.GetBytes(name).Length
            + sizeof(uint)); // Marshal.SizeOf(index)
    }
}
