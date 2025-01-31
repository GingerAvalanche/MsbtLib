using System.Text.RegularExpressions;

namespace MsbtLib.Controls.GrammarTags;

internal class Gender : Control
{
    public const string Tag = nameof(Gender);
    public const ushort TagType = 0x0005;
    private readonly ushort _paramSize;
    private readonly string _masc;
    private readonly string _femme;
    private readonly string _neut;

    public Gender(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        (_, _masc) = queue.DequeueCString();
        (_, _femme) = queue.DequeueCString();
        (_, _neut) = queue.DequeueCString();
    }

    public Gender(string str)
    {
        Regex pattern = new($@"<{Tag}\smasc='((?:\w|\d)*)'\sfemme='((?:\w|\d)*)'\sneut='((?:\w|\d)*)'\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException("I'll write this when I figure out what the unknown param(s) is/are!");
        }
        _masc = m.Groups[1].Value;
        _femme = m.Groups[2].Value;
        _neut = m.Groups[3].Value;
        _paramSize = (ushort)((_masc.Length + _femme.Length + _neut.Length) * 2 + 2);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(_paramSize + 8);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(GrammarTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_paramSize));
        buffer.AddRange(converter.GetCString(_masc));
        buffer.AddRange(converter.GetCString(_femme));
        buffer.AddRange(converter.GetCString(_neut));
        return buffer.ToArray();
    }

    public override string ToControlString()
    {
        return $"<{Tag} masc='{_masc}' femme='{_femme}' neut='{_neut}' />";
    }
}