using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.FourTags;

internal class FourZero : Control
{
    public const string Tag = nameof(FourZero);
    public const ushort TagType = 0x0000;
    private readonly ushort _paramSize;
    private readonly string _str;

    public FourZero(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        (ushort strSize, _str) = queue.DequeueCString();
        if (_paramSize != strSize + 2) throw new InvalidDataException("FourZero parameter size mismatch");
    }

    public FourZero(string str)
    {
        Regex pattern = new($@"<{Tag} str='((?:\w|\d)+)'\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException($"Proper usage: <{Tag} str='?' /> where ? is a string. Valid examples: <{Tag} str='Call' /> or <{Tag} str='' />");
        }
        _str = m.Groups[1].ToString();
        _paramSize = (ushort)(_str.Length * 2 + 2);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[_paramSize + 8];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(FourTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(_paramSize), 6);
        bytes.Merge(converter.GetCString(_str), 8);
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<{Tag} str='{_str}' />";
    }
}