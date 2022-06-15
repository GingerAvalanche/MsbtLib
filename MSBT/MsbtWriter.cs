using MsbtLib.Sections;
using System.Text;

namespace MsbtLib
{
    internal class MsbtWriter
    {
        public MSBT msbt;
        public WriteCounter writer;

        public MsbtWriter(MSBT msbt, BinaryWriter writer)
        {
            this.msbt = msbt;
            this.writer = new WriteCounter(writer);
        }

        public void WriteHeader()
        {
            writer.Write(msbt.header.magic);
            writer.Write(msbt.header.converter.Convert(0xFEFF));
            writer.Write(msbt.header.converter.Convert(msbt.header._unknown_1));
            writer.Write((byte)msbt.header.encoding);
            writer.Write(msbt.header.version);
            writer.Write(msbt.header.converter.Convert(msbt.header.section_count));
            writer.Write(msbt.header.converter.Convert(msbt.header._unknown_2));
            writer.Write(msbt.header.converter.Convert(msbt.header.file_size));
            writer.Write(msbt.header.padding);
        }

        public void WriteSection(SectionHeader section)
        {
            writer.Write(section.magic);
            writer.Write(msbt.header.converter.Convert(section.size));
            writer.Write(section.padding);
        }

        public void WriteGroup(Group group)
        {
            writer.Write(msbt.header.converter.Convert(group.label_count));
            writer.Write(msbt.header.converter.Convert(group.offset));
        }

        public void WriteLabel(Label label)
        {
            writer.Write((byte)label.Name.Length);
            writer.Write(Encoding.ASCII.GetBytes(label.Name));
            writer.Write(msbt.header.converter.Convert(label.Index));
        }

        public void WriteAto1(Ato1 ato1)
        {
            WriteSection(ato1.section);
            writer.Write(ato1._unknown);
            WritePadding();
        }

        public void WriteAtr1(Atr1 atr1)
        {
            WriteSection(atr1.section);
            writer.Write(msbt.header.converter.Convert((uint)atr1.Strings.Count));
            writer.Write(msbt.header.converter.Convert(atr1._unknown_1));
            if (atr1.Strings.Any(s => !string.IsNullOrEmpty(s)))
            {
                uint offset = sizeof(uint) * ((uint)atr1.Strings.Count + 2u); // sizeof(string_count) + sizeof(_unknown_1) + sizeof(uint) * string_count
                List<byte[]> raw_strings = atr1.Strings.Select(s => Util.StringToRaw(s, msbt.header.encoding, msbt.header.converter).ToArray()).ToList();
                foreach (byte[] s in raw_strings)
                {
                    writer.Write(msbt.header.converter.Convert(offset));
                    offset += (uint)s.Length;
                }
                foreach (byte[] s in raw_strings)
                {
                    writer.Write(s);
                }
            }
            WritePadding();
        }

        public void WriteLbl1(Lbl1 lbl1)
        {
            WriteSection(lbl1.section);
            List<List<Label>> groups = new();
            foreach (var _ in Enumerable.Range(0, (int)lbl1.GroupCount))
            {
                groups.Add(new());
            }
            lbl1.Labels.ForEach(l => groups[l.GroupNum()].Add(l));
            writer.Write(msbt.header.converter.Convert(lbl1.GroupCount));
            uint offset = (lbl1.GroupCount * 8) + 4;
            groups.ForEach(g => {
                WriteGroup(new((uint)g.Count, offset));
                offset += (uint)g.Select(l => (long)l.CalcSize()).Sum();
            });
            lbl1.Labels.ForEach(label => WriteLabel(label));
            WritePadding();
        }

        public void WriteNli1(Nli1 nli1)
        {
            WriteSection(nli1.section);
            if (nli1.section.size > 0)
            {
                writer.Write(msbt.header.converter.Convert(nli1.id_count));
                foreach (KeyValuePair<uint, uint> kvp in nli1.global_ids)
                {
                    writer.Write(msbt.header.converter.Convert(kvp.Value));
                    writer.Write(msbt.header.converter.Convert(kvp.Key));
                }
            }
            WritePadding();
        }

        public void WriteTsy1(Tsy1 tsy1)
        {
            WriteSection(tsy1.section);
            writer.Write(tsy1._unknown);
            WritePadding();
        }

        public void WriteTxt2(Txt2 txt2)
        {
            WriteSection(txt2.section);
            writer.Write(msbt.header.converter.Convert((uint)txt2.Strings.Count));
            uint total = 0;
            List<byte[]> raw_strings = txt2.Strings.Select(s => Util.StringToRaw(s, msbt.header.encoding, msbt.header.converter).ToArray()).ToList();
            foreach (byte[] s in raw_strings)
            {
                uint offset = (uint)txt2.Strings.Count * 4u + 4u + total;
                total += (uint)s.Length;
                writer.Write(msbt.header.converter.Convert(offset));
            }
            foreach (byte[] s in raw_strings)
            {
                writer.Write(s);
            }
            WritePadding();
        }

        public void WritePadding()
        {
            ulong rem = writer.written % MSBT.PADDING_LENGTH;
            if (rem == 0ul)
            {
                return;
            }
            writer.Write(new byte[MSBT.PADDING_LENGTH - rem].Fill(MSBT.PADDING_CHAR));
        }
    }

    internal class WriteCounter
    {
        public ulong written;
        public BinaryWriter writer;

        public WriteCounter(BinaryWriter writer)
        {
            this.writer = writer;
            written = 0ul;
        }

        public ulong Write(byte[] buf)
        {
            writer.Write(buf);
            ulong num = (ulong)buf.Length;
            written += num;
            return num;
        }

        public ulong Write(byte b)
        {
            writer.Write(b);
            written += 1ul;
            return 1ul;
        }

        public ulong Write(ushort u16)
        {
            writer.Write(u16);
            written += 2ul;
            return 2ul;
        }

        public ulong Write(uint u32)
        {
            writer.Write(u32);
            written += 4ul;
            return 4ul;
        }

        public ulong Write(ulong u64)
        {
            writer.Write(u64);
            written += 8ul;
            return 8ul;
        }
    }
}
