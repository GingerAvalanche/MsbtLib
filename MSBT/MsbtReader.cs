using MsbtLib.Sections;
using System.Text;

namespace MsbtLib
{
    class MsbtReader
    {
        private readonly ReadCounter reader;
        private readonly Header header;
        public Header Header { get => header; }

        public MsbtReader(BinaryReader reader)
        {
            this.reader = new ReadCounter(reader);
            header = ReadHeader();
        }

        public Header ReadHeader()
        {
            byte[] magic = reader.Read(8);
            if (Encoding.ASCII.GetString(magic) != MSBT.HEADER_MAGIC) {
                throw new MsbtException.InvalidMagicException(Encoding.ASCII.GetString(magic));
            }

            EndiannessConverter converter = reader.ReadU16() switch {
                0xFEFF => new(Endianness.Little),
                0xFFFE => new(Endianness.Big),
                _ => throw new MsbtException.InvalidBomException(),
            };

            ushort _unknown_1 = converter.Convert(reader.ReadU16());
            UTFEncoding encoding = reader.ReadByte() switch {
                0 => UTFEncoding.UTF8,
                1 => UTFEncoding.UTF16,
                _ => throw new MsbtException.InvalidEncodingException(),
            };

            byte version = reader.ReadByte();
            ushort section_count = converter.Convert(reader.ReadU16());
            ushort _unknown_2 = converter.Convert(reader.ReadU16());
            uint file_size = converter.Convert(reader.ReadU32());
            byte[] padding = reader.Read(10);
            return new Header(magic, converter, _unknown_1, encoding, version, section_count, _unknown_2, file_size, padding);
        }
        public Ato1 ReadAto1()
        {
            SectionHeader section = ReadSectionHeader();
            byte[] _unknown = reader.Read(section.size);
            return new(section, _unknown);
        }
        public Atr1 ReadAtr1()
        {
            SectionHeader section = ReadSectionHeader();
            uint string_count = header.converter.Convert(reader.ReadU32());
            uint _unknown_1 = header.converter.Convert(reader.ReadU32());
            List<string> strings = new();
            if (section.size > 8u) {
                List<uint> offsets = new();
                foreach (var _ in Enumerable.Range(0, (int)string_count)) {
                    offsets.Add(header.converter.Convert(reader.ReadU32()));
                }
                foreach (var i in Enumerable.Range(0, (int)string_count)) {
                    uint str_end = i == string_count - 1u ? section.size : offsets[i + 1];
                    strings.Add(Util.RawToString(reader.Read(str_end - offsets[i]).ToList(), header.encoding, header.converter));
                }
            }
            return new Atr1(header, section, string_count, _unknown_1, strings);
        }
        public Lbl1 ReadLbl1(MSBT msbt)
        {
            SectionHeader section = ReadSectionHeader();
            uint group_count = header.converter.Convert(reader.ReadU32());
            Lbl1 lbl1 = new(msbt, section);

            List<Group> groups = new();
            foreach (var _ in Enumerable.Range(0, (int)group_count)) {
                groups.Add(ReadGroup());
            }

            List<Label> labels = new(groups.Select(g => (int)g.label_count).Sum());
            foreach (var (group, i) in groups.Select((group, i) => (group, i))) {
                foreach (var _ in Enumerable.Range(0, (int)group.label_count)) {
                    ulong str_len = reader.ReadByte();
                    string name = Encoding.UTF8.GetString(reader.Read(str_len));
                    uint index = header.converter.Convert(reader.ReadU32());
                    uint checksum = (uint)i;
                    labels.Add(new Label(lbl1, name, index));
                }
            }
            lbl1.SetLabels(labels);

            return lbl1;
        }
        public Nli1 ReadNli1()
        {
            SectionHeader section = ReadSectionHeader();
            Dictionary<uint, uint> global_ids = new();
            uint id_count = 0;
            if (section.size > 0u) {
                id_count = header.converter.Convert(reader.ReadU32());
                foreach (var _ in Enumerable.Range(0, (int)id_count)) {
                    uint val = header.converter.Convert(reader.ReadU32());
                    uint key = header.converter.Convert(reader.ReadU32());
                    global_ids[key] = val;
                }
            }
            return new Nli1(section, id_count, global_ids);
        }
        public Tsy1 ReadTsy1()
        {
            SectionHeader section = ReadSectionHeader();
            byte[] _unknown = reader.Read(section.size);
            return new(section, _unknown);
        }
        public Txt2 ReadTxt2()
        {
            SectionHeader section = ReadSectionHeader();
            uint string_count = header.converter.Convert(reader.ReadU32());
            List<uint> offsets = new();
            List<string> strings = new();
            foreach (var _ in Enumerable.Range(0, (int)string_count)) {
                offsets.Add(header.converter.Convert(reader.ReadU32()));
            }
            foreach (var i in Enumerable.Range(0, (int)string_count)) {
                uint str_end = i == string_count - 1 ? section.size : offsets[i + 1];
                strings.Add(Util.RawToString(reader.Read(str_end - offsets[i]).ToList(), header.encoding, header.converter));
            }
            return new Txt2(header, section, strings);
        }
        public Group ReadGroup()
        {
            uint label_count = header.converter.Convert(reader.ReadU32());
            uint offset = header.converter.Convert(reader.ReadU32());
            return new(label_count, offset);
        }
        public SectionHeader ReadSectionHeader()
        {
            byte[] magic = reader.Read(4);
            uint size = header.converter.Convert(reader.ReadU32());
            byte[] padding = reader.Read(8);
            return new(magic, size, padding);
        }
        public bool SkipPadding()
        {
            for (var i = 0; i < (int)MSBT.PADDING_LENGTH; i++) {
                if (reader.HasReachedEOF()) {
                    return true;
                }
                else if (reader.ReadByte() != MSBT.PADDING_CHAR) {
                    reader.Seek(-1);
                    return true;
                }
            }
            return false;
        }
        public bool HasReachedEOF()
        {
            return reader.HasReachedEOF();
        }
        public byte[] Peek(int count)
        {
            return reader.Peek(count);
        }
    }
    internal class ReadCounter
    {
        ulong read;
        ulong length;
        BinaryReader reader;
        public ReadCounter(BinaryReader reader)
        {
            this.reader = reader;
            read = 0ul;
            length = (ulong)reader.BaseStream.Length;
        }
        public byte[] Peek(int count)
        {
            byte[] ret = reader.ReadBytes(count);
            reader.BaseStream.Seek(-count, SeekOrigin.Current);
            return ret;
        }
        public byte[] Read(ulong count)
        {
            if (read + count > length) {
                throw new EndOfStreamException();
            }
            read += count;
            return reader.ReadBytes((int)count);
        }
        public byte ReadByte()
        {
            if (read + 1ul > length) {
                throw new EndOfStreamException();
            }
            read += 1ul;
            return reader.ReadByte();
        }
        public ushort ReadU16()
        {
            if (read + 2ul > length) {
                throw new EndOfStreamException();
            }
            read += 2ul;
            return reader.ReadUInt16();
        }
        public uint ReadU32()
        {
            if (read + 4ul > length) {
                throw new EndOfStreamException();
            }
            read += 4ul;
            return reader.ReadUInt32();
        }
        public ulong ReadU64()
        {
            if (read + 8ul > length) {
                throw new EndOfStreamException();
            }
            read += 8ul;
            return reader.ReadUInt64();
        }
        public void Seek(int count)
        {
            read += (ulong)count;
            reader.BaseStream.Seek(count, SeekOrigin.Current);
        }
        public bool HasReachedEOF()
        {
            return read >= length;
        }
    }
}
