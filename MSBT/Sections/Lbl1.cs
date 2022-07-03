using System.Text;

namespace MsbtLib.Sections
{
    internal class Lbl1 : ICalculatesSize, IUpdates
    {
        private readonly List<Label> _labels;
        public readonly MSBT msbt;
        public SectionHeader section;
        public uint GroupCount { get => Math.Min(Convert.ToUInt32(((_labels.Count * 0.01f) + 1) * _labels.Count), 101); }
        public List<Label> Labels { get => _labels; }

        public Lbl1(MSBT msbt, SectionHeader section)
        {
            this.msbt = msbt;
            this.section = section;
            _labels = new();
        }

        public void SetLabels(IEnumerable<Label> labels)
        {
            _labels.Clear();
            foreach (Label label in labels)
            {
                _labels.Add(label);
            }
        }

        public ulong CalcSize() => section.CalcSize()
            + (ulong)(sizeof(uint) // Marshal.SizeOf(group_count)
            + (GroupCount * 8)
            + _labels.Select(l => (int)l.CalcSize()).Sum());

        public void Update()
        {
            section.size = (uint)(CalcSize() - section.CalcSize());
            _labels.Sort((l1, l2) => l1.Index.CompareTo(l2.Index));
            _labels.Sort((l1, l2) => l1.GroupNum() - l2.GroupNum());
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
        private string _name;

        private readonly Lbl1 _lbl1;
        public string Name { get => _name; }
        public uint Index { get; set; }

        public Label(Lbl1 lbl1, string name, uint index)
        {
            _lbl1 = lbl1;
            _name = name;
            Index = index;
        }

        public int GroupNum()
        {
            unchecked {
                return (int)(Encoding.UTF8.GetBytes(_name).ToList().Aggregate(0u, (hash, b) => hash * HASH_MAGIC + b) % _lbl1.GroupCount);
            }
        }

        public string Attribute {
            get => _lbl1.msbt.atr1 != null ? _lbl1.msbt.atr1.Strings[(int)Index] : string.Empty;
            set
            {
                if (_lbl1.msbt.atr1 != null)
                {
                    _lbl1.msbt.atr1.Strings[(int)Index] = value;
                }
            }
        }
        public string Value {
            get => _lbl1.msbt.txt2 != null ? _lbl1.msbt.txt2.Strings[(int)Index] : string.Empty;
            set
            {
                if (_lbl1.msbt.txt2 != null)
                {
                    _lbl1.msbt.txt2.Strings[(int)Index] = value;
                }
            }
        }

        public ulong CalcSize() => (ulong)(sizeof(byte) // name length
            + Encoding.ASCII.GetBytes(_name).Length
            + sizeof(uint)); // Marshal.SizeOf(index)
    }
}
