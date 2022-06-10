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

        public bool WriteHeader()
        {
            writer.Write(msbt.header.magic);
            writer.Write(0xFEFF, msbt.header.endianness);
            writer.Write(msbt.header._unknown_1, msbt.header.endianness);
            writer.Write((byte)msbt.header.encoding);
            writer.Write(msbt.header.version);
            writer.Write(msbt.header.section_count, msbt.header.endianness);
            writer.Write(msbt.header._unknown_2, msbt.header.endianness);
            writer.Write(msbt.header.file_size, msbt.header.endianness);
            writer.Write(msbt.header.padding);
            return true;
        }

        public bool WriteSection(SectionHeader section)
        {
            writer.Write(section.magic);
            writer.Write(section.size, msbt.header.endianness);
            writer.Write(section.padding);
            return true;
        }

        public bool WriteGroup(Group group)
        {
            writer.Write(group.label_count, msbt.header.endianness);
            writer.Write(group.offset, msbt.header.endianness);
            return true;
        }

        public bool WriteLabel(Label label)
        {
            writer.Write((byte)label.name.Length);
            writer.Write(Encoding.UTF8.GetBytes(label.name));
            writer.Write(label.index, msbt.header.endianness);
            return true;
        }

        public bool WriteAto1(Ato1 ato1)
        {
            WriteSection(ato1.section);
            writer.Write(ato1._unknown);
            WritePadding();
            return true;
        }

        public bool WriteAtr1(Atr1 atr1)
        {
            WriteSection(atr1.section);
            writer.Write(atr1.string_count, msbt.header.endianness);
            writer.Write(atr1._unknown_1, msbt.header.endianness);
            uint offset = sizeof(uint) * (atr1.string_count + 2u); // sizeof(string_count) + sizeof(_unknown_1) + sizeof(uint) * string_count
            foreach (List<byte> s in atr1.raw_strings) {
                writer.Write(offset, msbt.header.endianness);
                offset += (uint)s.Count;
            }
            foreach (List<byte> s in atr1.raw_strings) {
                writer.Write(s.Chunk((int)msbt.header.encoding + 1)
                    .SelectMany(c => c.ToEndianness(msbt.header.endianness))
                    .ToArray());
            }
            WritePadding();
            return true;
        }

        public bool WriteLbl1(Lbl1 lbl1)
        {
            WriteSection(lbl1.section);
            writer.Write(lbl1.group_count, msbt.header.endianness);
            lbl1.groups.ForEach(group => WriteGroup(group));
            lbl1.labels.ForEach(label => WriteLabel(label));
            WritePadding();
            return true;
        }

        public bool WriteNli1(Nli1 nli1)
        {
            WriteSection(nli1.section);
            if (nli1.section.size > 0) {
                writer.Write(nli1.id_count, msbt.header.endianness);
                foreach (KeyValuePair<uint, uint> kvp in nli1.global_ids) {
                    writer.Write(kvp.Value, msbt.header.endianness);
                    writer.Write(kvp.Key, msbt.header.endianness);
                }
            }
            WritePadding();
            return true;
        }

        public bool WriteTsy1(Tsy1 tsy1)
        {
            WriteSection(tsy1.section);
            writer.Write(tsy1._unknown);
            WritePadding();
            return true;
        }

        public bool WriteTxt2(Txt2 txt2)
        {
            WriteSection(txt2.section);
            writer.Write(txt2.string_count, msbt.header.endianness);
            uint total = 0;
            foreach (List<byte> s in txt2.raw_strings) {
                uint offset = txt2.string_count * 4u + 4u + total;
                total += (uint)s.Count;
                writer.Write(offset, msbt.header.endianness);
            }
            foreach (List<byte> s in txt2.raw_strings) {
                writer.Write(s.Chunk((int)msbt.header.encoding + 1)
                    .SelectMany(c => c.ToEndianness(msbt.header.endianness))
                    .ToArray());
            }
            WritePadding();
            return true;
        }

        public bool WritePadding()
        {
            ulong rem = writer.written % MSBT.PADDING_LENGTH;
            if (rem == 0ul) {
                return true;
            }
            writer.Write(new byte[MSBT.PADDING_LENGTH - rem].Fill(MSBT.PADDING_CHAR));
            return true;
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

        public ulong Write(ushort u16, Endianness endianness)
        {
            writer.Write(endianness == Endianness.Big ? Util.ReverseBytes(u16) : u16);
            written += 2ul;
            return 2ul;
        }

        public ulong Write(uint u32, Endianness endianness)
        {
            writer.Write(endianness == Endianness.Big ? Util.ReverseBytes(u32) : u32);
            written += 4ul;
            return 4ul;
        }

        public ulong Write(ulong u64, Endianness endianness)
        {
            writer.Write(endianness == Endianness.Big ? Util.ReverseBytes(u64) : u64);
            written += 8ul;
            return 8ul;
        }

        public void Flush() => writer.Flush();
    }
}
