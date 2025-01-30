using MsbtLib.Sections;
using System.Text;

namespace MsbtLib
{
    internal class MsbtWriter(Msbt msbt, BinaryWriter writer)
    {
        public readonly WriteCounter Writer = new(writer);

        public void WriteHeader()
        {
            Writer.Write(msbt.Header.Magic);
            Writer.Write(msbt.Header.Converter.Convert(0xFEFF));
            Writer.Write(msbt.Header.Converter.Convert(msbt.Header.Unknown1));
            Writer.Write((byte)msbt.Header.Encoding);
            Writer.Write(msbt.Header.Version);
            Writer.Write(msbt.Header.Converter.Convert(msbt.Header.SectionCount));
            Writer.Write(msbt.Header.Converter.Convert(msbt.Header.Unknown2));
            Writer.Write(msbt.Header.Converter.Convert(msbt.Header.FileSize));
            Writer.Write(msbt.Header.Padding);
        }

        private void WriteSection(SectionHeader section)
        {
            Writer.Write(section.Magic);
            Writer.Write(msbt.Header.Converter.Convert(section.Size));
            Writer.Write(section.Padding);
        }

        private void WriteGroup(Group group)
        {
            Writer.Write(msbt.Header.Converter.Convert(group.LabelCount));
            Writer.Write(msbt.Header.Converter.Convert(group.Offset));
        }

        private void WriteLabel(Label label)
        {
            Writer.Write((byte)label.Name.Length);
            Writer.Write(Encoding.ASCII.GetBytes(label.Name));
            Writer.Write(msbt.Header.Converter.Convert(label.Index));
        }

        public void WriteAto1(Ato1 ato1)
        {
            WriteSection(ato1.Section);
            Writer.Write(ato1.Unknown);
            WritePadding();
        }

        public void WriteAtr1(Atr1 atr1)
        {
            WriteSection(atr1.Section);
            Writer.Write(msbt.Header.Converter.Convert((uint)atr1.Strings.Count));
            Writer.Write(msbt.Header.Converter.Convert(atr1.Unknown1));
            if (atr1.Strings.Any(s => !string.IsNullOrEmpty(s)))
            {
                uint offset = sizeof(uint) * ((uint)atr1.Strings.Count + 2u); // sizeof(string_count) + sizeof(_unknown_1) + sizeof(uint) * string_count
                List<byte[]> rawStrings = atr1.Strings.Select(s => Util.StringToRaw(s, msbt.Header.Encoding, msbt.Header.Converter).ToArray()).ToList();
                foreach (byte[] s in rawStrings)
                {
                    Writer.Write(msbt.Header.Converter.Convert(offset));
                    offset += (uint)s.Length;
                }
                foreach (byte[] s in rawStrings)
                {
                    Writer.Write(s);
                }
            }
            WritePadding();
        }

        public void WriteLbl1(Lbl1 lbl1)
        {
            WriteSection(lbl1.Section);
            List<List<Label>> groups = new();
            foreach (var _ in Enumerable.Range(0, (int)lbl1.GroupCount))
            {
                groups.Add(new());
            }
            lbl1.Labels.ForEach(l => groups[l.GroupNum()].Add(l));
            Writer.Write(msbt.Header.Converter.Convert(lbl1.GroupCount));
            uint offset = (lbl1.GroupCount * 8) + 4;
            groups.ForEach(g => {
                WriteGroup(new((uint)g.Count, offset));
                offset += (uint)g.Select(l => (long)l.CalcSize()).Sum();
            });
            lbl1.Labels.ForEach(WriteLabel);
            WritePadding();
        }

        public void WriteNli1(Nli1 nli1)
        {
            WriteSection(nli1.Section);
            if (nli1.Section.Size > 0)
            {
                Writer.Write(msbt.Header.Converter.Convert(nli1.IdCount));
                foreach (KeyValuePair<uint, uint> kvp in nli1.GlobalIds)
                {
                    Writer.Write(msbt.Header.Converter.Convert(kvp.Value));
                    Writer.Write(msbt.Header.Converter.Convert(kvp.Key));
                }
            }
            WritePadding();
        }

        public void WriteTsy1(Tsy1 tsy1)
        {
            WriteSection(tsy1.Section);
            Writer.Write(tsy1.Unknown);
            WritePadding();
        }

        public void WriteTxt2(Txt2 txt2)
        {
            WriteSection(txt2.Section);
            Writer.Write(msbt.Header.Converter.Convert((uint)txt2.Strings.Count));
            uint total = 0;
            List<byte[]> rawStrings = txt2.Strings.Select(s => Util.StringToRaw(s, msbt.Header.Encoding, msbt.Header.Converter).ToArray()).ToList();
            foreach (byte[] s in rawStrings)
            {
                uint offset = (uint)txt2.Strings.Count * 4u + 4u + total;
                total += (uint)s.Length;
                Writer.Write(msbt.Header.Converter.Convert(offset));
            }
            foreach (byte[] s in rawStrings)
            {
                Writer.Write(s);
            }
            WritePadding();
        }

        private void WritePadding()
        {
            ulong rem = Writer.Written % Msbt.PaddingLength;
            if (rem == 0ul)
            {
                return;
            }
            Writer.Write(new byte[Msbt.PaddingLength - rem].Fill(Msbt.PaddingChar));
        }
    }

    internal class WriteCounter(BinaryWriter writer)
    {
        public ulong Written;
        public readonly BinaryWriter Writer = writer;

        public ulong Write(byte[] buf)
        {
            Writer.Write(buf);
            ulong num = (ulong)buf.Length;
            Written += num;
            return num;
        }

        public ulong Write(byte b)
        {
            Writer.Write(b);
            Written += 1ul;
            return 1ul;
        }

        public ulong Write(ushort u16)
        {
            Writer.Write(u16);
            Written += 2ul;
            return 2ul;
        }

        public ulong Write(uint u32)
        {
            Writer.Write(u32);
            Written += 4ul;
            return 4ul;
        }

        public ulong Write(ulong u64)
        {
            Writer.Write(u64);
            Written += 8ul;
            return 8ul;
        }
    }
}
