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

    public struct MsbtEntry
    {
        public string Attribute;
        public string Value;

        public MsbtEntry(string attribute, string value)
        {
            Attribute = attribute;
            Value = value;
        }
    }

    public class MSBT : ICalculatesSize, IUpdates
    {
        public static readonly string HEADER_MAGIC = "MsgStdBn";
        public const byte PADDING_CHAR = 0xAB;
        public const ulong PADDING_LENGTH = 16;
        public Header header;
        public List<SectionTag> section_order;
        internal Ato1? ato1;
        internal Atr1? atr1;
        internal Lbl1? lbl1;
        internal Nli1? nli1;
        internal Tsy1? tsy1;
        internal Txt2? txt2;

        public MSBT(Endianness endianness, UTFEncoding encoding)
        {
            header = new(Encoding.ASCII.GetBytes(HEADER_MAGIC), new(endianness), 0, encoding, 3, 0, 0, 0x20, new byte[10]);
            section_order = new List<SectionTag>();
        }
        public MSBT(Stream stream)
        {
            MsbtReader reader = new(new BinaryReader(stream));
            header = reader.Header;
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
            stream.Dispose();
        }

        public void Write(string file_name)
        {
            Update();
            MsbtWriter writer = new(this, new BinaryWriter(new FileStream(file_name, FileMode.Create, FileAccess.Write)));
            writer.WriteHeader();
#pragma warning disable CS8604 // Possible null reference argument.
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
#pragma warning restore CS8604 // Possible null reference argument.
            writer.Finish();
        }

        public byte[] Write()
        {
            Update();
            MsbtWriter writer = new(this, new BinaryWriter(new MemoryStream((int)header.file_size)));
            writer.WriteHeader();
#pragma warning disable CS8604 // Possible null reference argument.
            foreach (SectionTag tag in section_order)
            {
                switch (tag)
                {
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
#pragma warning restore CS8604 // Possible null reference argument.
            byte[] bytes = ((MemoryStream)writer.writer.writer.BaseStream).ToArray();
            writer.Finish();
            return bytes;
        }

        public void CreateAto1()
        {
            if (ato1 != null)
            {
                return;
            }
            ato1 = new(new(Encoding.ASCII.GetBytes("ATO1"), 0), Array.Empty<byte>());
            section_order.Add(SectionTag.Ato1);
            header.section_count += 1;
        }
        public void CreateAtr1()
        {
            if (atr1 != null)
            {
                return;
            }
            atr1 = new(header, new(Encoding.ASCII.GetBytes("ATR1"), 8), 0, 0, new());
            section_order.Add(SectionTag.Atr1);
            header.section_count += 1;
            if (lbl1 != null)
            {
                atr1.SetStrings(lbl1.Labels.Select(l => ""));
            }
        }
        public void CreateLbl1()
        {
            if (lbl1 != null)
            {
                return;
            }
            lbl1 = new(this, new(Encoding.ASCII.GetBytes("LBL1"), 12));
            section_order.Add(SectionTag.Lbl1);
            header.section_count += 1;
        }
        public void CreateTxt2()
        {
            if (txt2 != null)
            {
                return;
            }
            txt2 = new(header, new(Encoding.ASCII.GetBytes("TXT2"), 8), new());
            section_order.Add(SectionTag.Txt2);
            header.section_count += 1;
            if (lbl1 != null)
            {
                txt2.SetStrings(lbl1.Labels.Select(l => ""));
            }
        }

        public Dictionary<string, MsbtEntry> GetTexts()
        {
            if (lbl1 == null || atr1 == null || txt2 == null)
            {
                throw new Exception("This MSBT does not contain texts.");
            }
            Dictionary<string, MsbtEntry> texts = new();
            foreach (Label label in lbl1.Labels) {
                texts.Add(label.Name, new(atr1.Strings[(int)label.Index], txt2.Strings[(int)label.Index]));
            }
            return texts;
        }

        public void SetTexts(Dictionary<string, MsbtEntry> texts)
        {
            if (lbl1 == null || txt2 == null)
            {
                throw new Exception(@"This MSBT does not support texts. Use CreateLbl1() and/or 
                    CreateTxt2() to make it support texts.");
            }
            if (texts.Values.Any(e => !string.IsNullOrEmpty(e.Attribute)) && atr1 == null)
            {
                throw new Exception(@"This MSBT has no attribute section but was given an attribute. 
                    Use CreateAtr1() to give it an attribute section if you want it to have one.");
            }
            List<string> new_keys = texts.Keys.Except(lbl1.Labels.Select(l => l.Name)).ToList();
            foreach (string old_key in texts.Keys.Except(new_keys))
            {
                Label label = lbl1.Labels.First(l => l.Name == old_key);
                if (atr1 != null)
                {
                    label.Attribute = texts[old_key].Attribute;
                }
                label.Value = texts[old_key].Value;
            }
            foreach (string new_key in new_keys)
            {
                uint index = 0;
                if (atr1 != null)
                {
                    atr1.AddString(texts[new_key].Attribute);
                }
                index = txt2.AddString(texts[new_key].Value);
                lbl1.Labels.Add(new(lbl1, new_key, index));
            }
            lbl1.Update();
            if (atr1 != null)
            {
                atr1.Update();
            }
            txt2.Update();
        }

        public void SetEncoding(UTFEncoding encoding) => header.encoding = encoding;
        public void SetEndianness(Endianness endianness) => header.converter.SetEndianness(endianness);
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
                + (lbl1 != null ? PlusPadding(lbl1.CalcSize()) : 0)
                + (nli1 != null ? PlusPadding(nli1.CalcSize()) : 0)
                + (ato1 != null ? PlusPadding(ato1.CalcSize()) : 0)
                + (atr1 != null ? PlusPadding(atr1.CalcSize()) : 0)
                + (tsy1 != null ? PlusPadding(tsy1.CalcSize()) : 0)
                + (txt2 != null ? PlusPadding(txt2.CalcSize()) : 0);
        }
    }
}
