using MsbtLib.Sections;
using JetBrains.Annotations;
using System.Text;

namespace MsbtLib
{
    public enum SectionTag
    {
        Ato1,
        Atr1,
        Lbl1,
        Nli1,
        Tsy1,
        Txt2
    }

    public enum Endianness
    {
        Big,
        Little
    }

    public enum UTFEncoding
    {
        UTF8,
        UTF16
    }

    public class MSBT : ICalculatesSize, IUpdates
    {
        public static readonly string HEADER_MAGIC = "MsgStdBn";
        public const byte PADDING_CHAR = 0xAB;
        public const ulong PADDING_LENGTH = 16;
        public Header header;
        public List<SectionTag> section_order;
        [CanBeNull]
        internal Ato1 ato1;
        [CanBeNull]
        internal Atr1 atr1;
        [CanBeNull]
        internal Lbl1 lbl1;
        [CanBeNull]
        internal Nli1 nli1;
        [CanBeNull]
        internal Tsy1 tsy1;
        [CanBeNull]
        internal Txt2 txt2;

        public MSBT(Stream stream)
        {
            MsbtReader reader = new(new BinaryReader(stream));
            section_order = new();
            while (true) {
                if (reader.HasReachedEOF()) {
                    break;
                }
                switch (Encoding.UTF8.GetString(reader.Peek(4))) {
                    case "ATO1":
                        ato1 = reader.ReadAto1();
                        section_order.Add(SectionTag.Ato1);
                        break;
                    case "ATR1":
                        atr1 = reader.ReadAtr1();
                        section_order.Add(SectionTag.Atr1);
                        break;
                    case "LBL1":
                        lbl1 = reader.ReadLbl1(this);
                        section_order.Add(SectionTag.Lbl1);
                        break;
                    case "NLI1":
                        nli1 = reader.ReadNli1();
                        section_order.Add(SectionTag.Nli1);
                        break;
                    case "TSY1":
                        tsy1 = reader.ReadTsy1();
                        section_order.Add(SectionTag.Tsy1);
                        break;
                    case "TXT2":
                        txt2 = reader.ReadTxt2();
                        section_order.Add(SectionTag.Txt2);
                        break;
                }
                reader.SkipPadding();
            }
        }

        public Stream Write()
        {
            MsbtWriter writer = new(this, new BinaryWriter(new MemoryStream()));
            writer.WriteHeader();
            foreach (SectionTag tag in section_order) {
                switch (tag) {
                    case SectionTag.Ato1:
                        writer.WriteAto1(ato1);
                        break;
                    case SectionTag.Atr1:
                        writer.WriteAtr1(atr1);
                        break;
                    case SectionTag.Lbl1:
                        writer.WriteLbl1(lbl1);
                        break;
                    case SectionTag.Nli1:
                        writer.WriteNli1(nli1);
                        break;
                    case SectionTag.Tsy1:
                        writer.WriteTsy1(tsy1);
                        break;
                    case SectionTag.Txt2:
                        writer.WriteTxt2(txt2);
                        break;
                }
            }
            return writer.writer.writer.BaseStream;
        }

        public Dictionary<string, string> GetTexts()
        {
            Dictionary<string, string> texts = new();
            List<string> strings = txt2.Strings();
            foreach (Label label in lbl1.labels) {
                texts.Add(label.name, strings[(int)label.index]);
            }
            return texts;
        }

        public void SetTexts(Dictionary<string, string> texts)
        {
            List<string> strings = new();
            List<Label> labels = new();
            foreach (KeyValuePair<string, string> kvp in texts) {
                labels.Add(new Label(lbl1, kvp.Value, (uint)strings.Count, Label.GenerateChecksum(kvp.Value, lbl1.group_count)));
                strings.Add(kvp.Key);
            }
            lbl1.labels = labels;
            lbl1.Update();
            txt2.SetStrings(strings);
            txt2.Update();
        }

        public void SetEncoding(UTFEncoding encoding) => header.encoding = encoding;
        public void SetEndianness(Endianness endianness) => header.endianness = endianness;
        public ulong PlusPadding(ulong size)
        {
            ulong rem = size % 16ul;
            if (rem > 0) {
                return size + (16ul - rem);
            }
            return size;
        }

        public void Update()
        {
            header.file_size = (uint)CalcSize();
            header.section_count = (ushort)section_order.Count;
        }

        public ulong CalcSize()
        {
            return header.CalcSize()
                + PlusPadding(lbl1.CalcSize())
                + PlusPadding(nli1.CalcSize())
                + PlusPadding(ato1.CalcSize())
                + PlusPadding(atr1.CalcSize())
                + PlusPadding(tsy1.CalcSize())
                + PlusPadding(txt2.CalcSize());
        }
    }
}
