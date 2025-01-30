using System.Text;

namespace MsbtLib.Sections
{
    internal class Lbl1(Msbt msbt, SectionHeader section) : ICalculatesSize, IUpdates
    {
        private readonly List<Label> _labels = new();
        public readonly Msbt Msbt = msbt;
        public readonly SectionHeader Section = section;
        public uint GroupCount => Math.Min(Convert.ToUInt32(((_labels.Count * 0.01f) + 1) * _labels.Count), 101);
        public List<Label> Labels => _labels;

        public void SetLabels(IEnumerable<Label> labels)
        {
            _labels.Clear();
            foreach (Label label in labels)
            {
                _labels.Add(label);
            }
        }

        public ulong CalcSize() => Section.CalcSize()
            + (ulong)(sizeof(uint) // Marshal.SizeOf(group_count)
            + (GroupCount * 8)
            + _labels.Select(l => (int)l.CalcSize()).Sum());

        public void Update()
        {
            Section.Size = (uint)(CalcSize() - Section.CalcSize());
            _labels.Sort((l1, l2) => l1.Index.CompareTo(l2.Index));
            _labels.Sort((l1, l2) => l1.GroupNum() - l2.GroupNum());
            if (Msbt.Txt2 != null) {
                Msbt.Txt2.Update();
            }
        }
    }
    class Group(uint labelCount, uint offset) : ICalculatesSize
    {
        public readonly uint LabelCount = labelCount;
        public readonly uint Offset = offset;

        public ulong CalcSize() => sizeof(uint) * 2; // Marshal.SizeOf(label_count) + Marshal.SizeOf(offset)
    }
    internal class Label(Lbl1 lbl1, string name, uint index) : ICalculatesSize
    {
        private const uint HashMagic = 0x492;

        public string Name => name;
        public uint Index { get; set; } = index;

        public int GroupNum()
        {
            unchecked {
                return (int)(Encoding.UTF8.GetBytes(name).ToList().Aggregate(0u, (hash, b) => hash * HashMagic + b) % lbl1.GroupCount);
            }
        }

        public string Attribute {
            get => lbl1.Msbt.Atr1 != null ? lbl1.Msbt.Atr1.Strings[(int)Index] : string.Empty;
            set
            {
                if (lbl1.Msbt.Atr1 != null)
                {
                    lbl1.Msbt.Atr1.Strings[(int)Index] = value;
                }
            }
        }
        public string Value {
            get => lbl1.Msbt.Txt2 != null ? lbl1.Msbt.Txt2.Strings[(int)Index] : string.Empty;
            set
            {
                if (lbl1.Msbt.Txt2 != null)
                {
                    lbl1.Msbt.Txt2.Strings[(int)Index] = value;
                }
            }
        }

        public ulong CalcSize() => (ulong)(sizeof(byte) // name length
            + Encoding.ASCII.GetBytes(name).Length
            + sizeof(uint)); // Marshal.SizeOf(index)
    }
}
