using System.Text.RegularExpressions;

namespace MsbtLib.Controls.GrammarTags;

internal class Pluralize : Control
{
    public const string Tag = nameof(Pluralize);
    public const ushort TagType = 0x0006;
    private readonly ushort _paramSize;
    private readonly string _param0;
    private readonly string _param1;
    private readonly string _param2;

    public Pluralize(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        (_, _param0) = queue.DequeueCString();
        (_, _param1) = queue.DequeueCString();
        (_, _param2) = queue.DequeueCString();
    }

    public Pluralize(string str)
    {
        Regex pattern = new($@"<{Tag}\s0='((?:\w|\d)*)'\s1='((?:\w|\d)*)'\s2='((?:\w|\d)*)'\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException("I'll write this when I figure out what the unknown param(s) is/are!");
        }
        _param0 = m.Groups[1].Value;
        _param1 = m.Groups[2].Value;
        _param2 = m.Groups[3].Value;
        _paramSize = (ushort)((_param0.Length + _param1.Length + _param2.Length) * 2 + 2);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(_paramSize + 8);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(GrammarTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_paramSize));
        buffer.AddRange(converter.GetCString(_param0));
        buffer.AddRange(converter.GetCString(_param1));
        buffer.AddRange(converter.GetCString(_param2));
        return buffer.ToArray();
    }

    public override string ToControlString()
    {
        return $"<{Tag} 0='{_param0}' 1='{_param1}' 2='{_param2}' />";
    }
}