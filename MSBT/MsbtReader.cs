using MsbtLib.Sections;
using System.Text;

namespace MsbtLib
{
    class MsbtReader
    {
        private readonly ReadCounter _reader;
        public Header Header { get; }

        public MsbtReader(BinaryReader reader)
        {
            _reader = new ReadCounter(reader);
            Header = ReadHeader();
        }

        private Header ReadHeader()
        {
            byte[] magic = _reader.Read(8);
            if (Encoding.ASCII.GetString(magic) != Msbt.HeaderMagic) {
                throw new MsbtException.InvalidMagicException(Encoding.ASCII.GetString(magic));
            }

            EndiannessConverter converter = _reader.ReadU16() switch {
                0xFEFF => new(Endianness.Little),
                0xFFFE => new(Endianness.Big),
                _ => throw new MsbtException.InvalidBomException(),
            };

            ushort unknown1 = converter.Convert(_reader.ReadU16());
            UtfEncoding encoding = _reader.ReadByte() switch {
                0 => UtfEncoding.Utf8,
                1 => UtfEncoding.Utf16,
                _ => throw new MsbtException.InvalidEncodingException(),
            };

            byte version = _reader.ReadByte();
            ushort sectionCount = converter.Convert(_reader.ReadU16());
            ushort unknown2 = converter.Convert(_reader.ReadU16());
            uint fileSize = converter.Convert(_reader.ReadU32());
            byte[] padding = _reader.Read(10);
            return new Header(magic, converter, unknown1, encoding, version, sectionCount, unknown2, fileSize, padding);
        }
        public Ato1 ReadAto1()
        {
            SectionHeader section = ReadSectionHeader();
            byte[] unknown = _reader.Read(section.Size);
            return new(section, unknown);
        }
        public Atr1 ReadAtr1()
        {
            SectionHeader section = ReadSectionHeader();
            uint stringCount = Header.Converter.Convert(_reader.ReadU32());
            uint unknown1 = Header.Converter.Convert(_reader.ReadU32());
            List<string> strings = new();
            if (section.Size > 8u) {
                List<uint> offsets = new();
                foreach (var _ in Enumerable.Range(0, (int)stringCount)) {
                    offsets.Add(Header.Converter.Convert(_reader.ReadU32()));
                }
                foreach (var i in Enumerable.Range(0, (int)stringCount)) {
                    uint strEnd = i == stringCount - 1u ? section.Size : offsets[i + 1];
                    strings.Add(Util.RawToString(_reader.Read(strEnd - offsets[i]).ToList(), Header.Encoding, Header.Converter));
                }
            }
            return new Atr1(Header, section, stringCount, unknown1, strings);
        }
        public Lbl1 ReadLbl1(Msbt msbt)
        {
            SectionHeader section = ReadSectionHeader();
            uint groupCount = Header.Converter.Convert(_reader.ReadU32());
            Lbl1 lbl1 = new(msbt, section);

            List<Group> groups = new();
            foreach (var _ in Enumerable.Range(0, (int)groupCount)) {
                groups.Add(ReadGroup());
            }

            List<Label> labels = new(groups.Select(g => (int)g.LabelCount).Sum());
            foreach (var (group, _) in groups.Select((group, i) => (group, i))) {
                foreach (var _ in Enumerable.Range(0, (int)group.LabelCount)) {
                    ulong strLen = _reader.ReadByte();
                    string name = Encoding.UTF8.GetString(_reader.Read(strLen));
                    uint index = Header.Converter.Convert(_reader.ReadU32());
                    labels.Add(new Label(lbl1, name, index));
                }
            }
            lbl1.SetLabels(labels);

            return lbl1;
        }
        public Nli1 ReadNli1()
        {
            SectionHeader section = ReadSectionHeader();
            Dictionary<uint, uint> globalIds = new();
            uint idCount = 0;
            if (section.Size > 0u) {
                idCount = Header.Converter.Convert(_reader.ReadU32());
                foreach (var _ in Enumerable.Range(0, (int)idCount)) {
                    uint val = Header.Converter.Convert(_reader.ReadU32());
                    uint key = Header.Converter.Convert(_reader.ReadU32());
                    globalIds[key] = val;
                }
            }
            return new Nli1(section, idCount, globalIds);
        }
        public Tsy1 ReadTsy1()
        {
            SectionHeader section = ReadSectionHeader();
            byte[] unknown = _reader.Read(section.Size);
            return new(section, unknown);
        }
        public Txt2 ReadTxt2()
        {
            SectionHeader section = ReadSectionHeader();
            uint stringCount = Header.Converter.Convert(_reader.ReadU32());
            List<uint> offsets = new();
            List<string> strings = new();
            foreach (var _ in Enumerable.Range(0, (int)stringCount)) {
                offsets.Add(Header.Converter.Convert(_reader.ReadU32()));
            }
            foreach (var i in Enumerable.Range(0, (int)stringCount)) {
                uint strEnd = i == stringCount - 1 ? section.Size : offsets[i + 1];
                strings.Add(Util.RawToString(_reader.Read(strEnd - offsets[i]).ToList(), Header.Encoding, Header.Converter));
            }
            return new Txt2(Header, section, strings);
        }

        private Group ReadGroup()
        {
            uint labelCount = Header.Converter.Convert(_reader.ReadU32());
            uint offset = Header.Converter.Convert(_reader.ReadU32());
            return new(labelCount, offset);
        }

        private SectionHeader ReadSectionHeader()
        {
            byte[] magic = _reader.Read(4);
            uint size = Header.Converter.Convert(_reader.ReadU32());
            byte[] padding = _reader.Read(8);
            return new(magic, size, padding);
        }
        public bool SkipPadding()
        {
            for (var i = 0; i < (int)Msbt.PaddingLength; i++) {
                if (_reader.HasReachedEof()) {
                    return true;
                }
                else if (_reader.ReadByte() != Msbt.PaddingChar) {
                    _reader.Seek(-1);
                    return true;
                }
            }
            return false;
        }
        public bool HasReachedEof()
        {
            return _reader.HasReachedEof();
        }
        public byte[] Peek(int count)
        {
            return _reader.Peek(count);
        }
    }
    internal class ReadCounter(BinaryReader reader)
    {
        private ulong _read;
        private readonly ulong _length = (ulong)reader.BaseStream.Length;

        public byte[] Peek(int count)
        {
            byte[] ret = reader.ReadBytes(count);
            reader.BaseStream.Seek(-count, SeekOrigin.Current);
            return ret;
        }
        public byte[] Read(ulong count)
        {
            if (_read + count > _length) {
                throw new EndOfStreamException();
            }
            _read += count;
            return reader.ReadBytes((int)count);
        }
        public byte ReadByte()
        {
            if (_read + 1ul > _length) {
                throw new EndOfStreamException();
            }
            _read += 1ul;
            return reader.ReadByte();
        }
        public ushort ReadU16()
        {
            if (_read + 2ul > _length) {
                throw new EndOfStreamException();
            }
            _read += 2ul;
            return reader.ReadUInt16();
        }
        public uint ReadU32()
        {
            if (_read + 4ul > _length) {
                throw new EndOfStreamException();
            }
            _read += 4ul;
            return reader.ReadUInt32();
        }
        public ulong ReadU64()
        {
            if (_read + 8ul > _length) {
                throw new EndOfStreamException();
            }
            _read += 8ul;
            return reader.ReadUInt64();
        }
        public void Seek(int count)
        {
            _read += (ulong)count;
            reader.BaseStream.Seek(count, SeekOrigin.Current);
        }
        public bool HasReachedEof()
        {
            return _read >= _length;
        }
    }
}
