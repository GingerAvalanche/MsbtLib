using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.FourTags;

internal class FourZero : Control
{
    public const string Tag = nameof(FourZero);
    public const ushort TagType = 0x0000;
    private readonly ushort _paramSize;
    private readonly ushort _strSize;
    private readonly string _str;

    public FourZero(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        _strSize = queue.DequeueU16();
        if (_paramSize == 0)
        {
            _str = "";
            return;
        }
        if (_paramSize != _strSize + 2) throw new InvalidDataException("FourZero parameter size mismatch");
        StringBuilder b = new();
        for (int i = 0; i < _strSize / 2; i++)
        {
            b.Append(queue.DequeueU8());
        }
        _str = b.ToString();
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
        _strSize = (ushort)(_str.Length * 2);
        _paramSize = (ushort)(_strSize + 2);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[_paramSize + 8];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(FourTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(_paramSize), 6);
        bytes.Merge(converter.GetBytes(_strSize), 8);
        bytes.Merge(Encoding.Unicode.GetBytes(_str), 10);
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<{Tag} str='{_str}' />";
    }
}