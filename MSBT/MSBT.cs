using MsbtLib.Sections;
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

    public enum UtfEncoding
    {
        Utf8,
        Utf16
    }

    public class MsbtEntry(string attribute, string value)
    {
        public string? Attribute { get; set; } = attribute is "" ? null : attribute;
        public string? Value { get; set; } = value is "" ? null : value;
    }

    public class Msbt : ICalculatesSize, IUpdates
    {
        public const string HeaderMagic = "MsgStdBn";
        public const byte PaddingChar = 0xAB;
        public const ulong PaddingLength = 16;
        public Header Header;
        public List<SectionTag> SectionOrder;
        internal Ato1? Ato1;
        internal Atr1? Atr1;
        internal Lbl1? Lbl1;
        internal Nli1? Nli1;
        internal Tsy1? Tsy1;
        internal Txt2? Txt2;

        public Msbt(Endianness endianness, UtfEncoding encoding)
        {
            Header = new(Encoding.ASCII.GetBytes(HeaderMagic), new(endianness), 0, encoding, 3, 0, 0, 0x20, new byte[10]);
            SectionOrder = new List<SectionTag>();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Msbt(byte[] data)
        {
            using MemoryStream stream = new(data);
            Load(stream);
        }
        public Msbt(Stream stream)
        {
            Load(stream);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private void Load(Stream stream)
        {
            using BinaryReader bReader = new(stream);
            MsbtReader reader = new(bReader);
            Header = reader.Header;
            SectionOrder = new();
            while (true)
            {
                if (reader.HasReachedEof())
                {
                    break;
                }
                switch (Encoding.UTF8.GetString(reader.Peek(4)))
                {
                    case "ATO1":
                        Ato1 = reader.ReadAto1();
                        SectionOrder.Add(SectionTag.Ato1);
                        break;
                    case "ATR1":
                        Atr1 = reader.ReadAtr1();
                        SectionOrder.Add(SectionTag.Atr1);
                        break;
                    case "LBL1":
                        Lbl1 = reader.ReadLbl1(this);
                        SectionOrder.Add(SectionTag.Lbl1);
                        break;
                    case "NLI1":
                        Nli1 = reader.ReadNli1();
                        SectionOrder.Add(SectionTag.Nli1);
                        break;
                    case "TSY1":
                        Tsy1 = reader.ReadTsy1();
                        SectionOrder.Add(SectionTag.Tsy1);
                        break;
                    case "TXT2":
                        Txt2 = reader.ReadTxt2();
                        SectionOrder.Add(SectionTag.Txt2);
                        break;
                }
                reader.SkipPadding();
            }
        }

        public void Write(string fileName)
        {
            Update();
            using FileStream fStream = new(fileName, FileMode.Create, FileAccess.Write);
            using BinaryWriter bWriter = new(fStream);
            MsbtWriter writer = new(this, bWriter);
            writer.WriteHeader();
            foreach (SectionTag tag in SectionOrder) {
                switch (tag) {
                    case SectionTag.Ato1:
                        writer.WriteAto1(Ato1!);
                        break;
                    case SectionTag.Atr1:
                        writer.WriteAtr1(Atr1!);
                        break;
                    case SectionTag.Lbl1:
                        writer.WriteLbl1(Lbl1!);
                        break;
                    case SectionTag.Nli1:
                        writer.WriteNli1(Nli1!);
                        break;
                    case SectionTag.Tsy1:
                        writer.WriteTsy1(Tsy1!);
                        break;
                    case SectionTag.Txt2:
                        writer.WriteTxt2(Txt2!);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public byte[] Write()
        {
            Update();
            using MemoryStream mStream = new((int)Header.FileSize);
            using BinaryWriter bWriter = new(mStream);
            MsbtWriter writer = new(this, bWriter);
            writer.WriteHeader();
#pragma warning disable CS8604 // Possible null reference argument.
            foreach (SectionTag tag in SectionOrder)
            {
                switch (tag)
                {
                    case SectionTag.Ato1:
                        writer.WriteAto1(Ato1);
                        break;
                    case SectionTag.Atr1:
                        writer.WriteAtr1(Atr1);
                        break;
                    case SectionTag.Lbl1:
                        writer.WriteLbl1(Lbl1);
                        break;
                    case SectionTag.Nli1:
                        writer.WriteNli1(Nli1);
                        break;
                    case SectionTag.Tsy1:
                        writer.WriteTsy1(Tsy1);
                        break;
                    case SectionTag.Txt2:
                        writer.WriteTxt2(Txt2);
                        break;
                }
            }
#pragma warning restore CS8604 // Possible null reference argument.
            return ((MemoryStream)writer.Writer.Writer.BaseStream).ToArray();
        }

        public void CreateAto1()
        {
            if (Ato1 != null)
            {
                return;
            }
            Ato1 = new(new("ATO1"u8.ToArray(), 0), []);
            SectionOrder.Add(SectionTag.Ato1);
            Header.SectionCount += 1;
        }
        public void CreateAtr1()
        {
            if (Atr1 != null)
            {
                return;
            }
            Atr1 = new(Header, new("ATR1"u8.ToArray(), 8), 0, 0, new());
            SectionOrder.Add(SectionTag.Atr1);
            Header.SectionCount += 1;
        }
        public void CreateLbl1()
        {
            if (Lbl1 != null)
            {
                return;
            }
            Lbl1 = new(this, new(Encoding.ASCII.GetBytes("LBL1"), 12));
            SectionOrder.Add(SectionTag.Lbl1);
            Header.SectionCount += 1;
        }
        public void CreateTxt2()
        {
            if (Txt2 != null)
            {
                return;
            }
            Txt2 = new(Header, new(Encoding.ASCII.GetBytes("TXT2"), 8), new());
            SectionOrder.Add(SectionTag.Txt2);
            Header.SectionCount += 1;
            if (Lbl1 != null)
            {
                Txt2.SetStrings(Lbl1.Labels.Select(_ => ""));
            }
        }

        public Dictionary<string, MsbtEntry> GetTexts()
        {
            if (Lbl1 == null || Txt2 == null)
            {
                throw new Exception("This MSBT does not contain texts.");
            }
            Dictionary<string, MsbtEntry> texts = new();
            foreach (Label label in Lbl1.Labels) {
                string atr = Atr1?.Strings[(int)label.Index] ?? string.Empty;
                texts.Add(label.Name, new(atr, Txt2.Strings[(int)label.Index]));
            }
            return texts;
        }

        public void SetTexts(Dictionary<string, MsbtEntry> texts)
        {
            if (Lbl1 == null || Txt2 == null)
            {
                throw new Exception(@"This MSBT does not support texts. Use CreateLbl1() and/or 
                    CreateTxt2() to make it support texts.");
            }
            if (texts.Values.Any(e => !string.IsNullOrEmpty(e.Attribute)) && Atr1 == null)
            {
                throw new Exception(@"This MSBT has no attribute section but was given an attribute. 
                    Use CreateAtr1() to give it an attribute section if you want it to have one.");
            }
            List<string> newKeys = texts.Keys.Except(Lbl1.Labels.Select(l => l.Name)).ToList();
            foreach (string oldKey in texts.Keys.Except(newKeys))
            {
                Label label = Lbl1.Labels.First(l => l.Name == oldKey);
                if (Atr1 != null)
                {
                    label.Attribute = texts[oldKey].Attribute ?? string.Empty;
                }
                label.Value = texts[oldKey].Value ?? string.Empty;
            }
            foreach (string newKey in newKeys)
            {
                Atr1?.AddString(texts[newKey].Attribute ?? string.Empty);
                var index = Txt2.AddString(texts[newKey].Value ?? string.Empty);
                Lbl1.Labels.Add(new(Lbl1, newKey, index));
            }
            Lbl1.Update();
            Atr1?.Update();
            Txt2.Update();
        }

        public void SetEncoding(UtfEncoding encoding) => Header.Encoding = encoding;
        public void SetEndianness(Endianness endianness) => Header.Converter.Endianness = endianness;

        private static ulong PlusPadding(ulong size)
        {
            ulong rem = size % 16ul;
            if (rem > 0) {
                return size + (16ul - rem);
            }
            return size;
        }

        public void Update()
        {
            Header.FileSize = (uint)CalcSize();
            Header.SectionCount = (ushort)SectionOrder.Count;
        }

        public ulong CalcSize()
        {
            return Header.CalcSize()
                + (Lbl1 != null ? PlusPadding(Lbl1.CalcSize()) : 0)
                + (Nli1 != null ? PlusPadding(Nli1.CalcSize()) : 0)
                + (Ato1 != null ? PlusPadding(Ato1.CalcSize()) : 0)
                + (Atr1 != null ? PlusPadding(Atr1.CalcSize()) : 0)
                + (Tsy1 != null ? PlusPadding(Tsy1.CalcSize()) : 0)
                + (Txt2 != null ? PlusPadding(Txt2.CalcSize()) : 0);
        }
    }
}
