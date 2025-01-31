using System.Text.RegularExpressions;

namespace MsbtLib.Controls.GrammarTags;

internal class LongVowel2 : Control
{
    public const string Tag = nameof(LongVowel2);
    public const ushort TagType = 0x0008;
    private readonly ushort _paramSize;
    private readonly string _param0;
    private readonly string _param1;

    public LongVowel2(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        (_, _param0) = queue.DequeueCString();
        (_, _param1) = queue.DequeueCString();
    }

    public LongVowel2(string str)
    {
        Regex pattern = new($@"<{Tag}\s0='((?:\w|\d)*)'\s1='((?:\w|\d)*)'\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException("<LongVowel2 0='[str]' 1='[str]' />");
        }
        _param0 = m.Groups[1].Value;
        _param1 = m.Groups[2].Value;
        _paramSize = (ushort)((_param0.Length + _param1.Length) * 2 + 2);
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
        return buffer.ToArray();
    }

    public override string ToControlString()
    {
        return $"<{Tag} 0='{_param0}' 1='{_param1}' />";
    }
}