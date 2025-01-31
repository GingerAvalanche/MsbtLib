using System.Text.RegularExpressions;

namespace MsbtLib.Controls.GrammarTags;

internal class Info : Control
{
    public const string Tag = nameof(Info);
    public const ushort TagType = 0x0000;
    private const ushort ParamSize = 0x0004;
    private readonly byte _gender;
    private readonly byte _definite;
    private readonly byte _indefinite;
    private readonly byte _plural;

    public Info(ref VariableByteQueue queue)
    {
        if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Info parameter size mismatch");
        _gender = queue.DequeueU8();
        _definite = queue.DequeueU8();
        _indefinite = queue.DequeueU8();
        _plural = queue.DequeueU8();
    }

    public Info(string str)
    {
        Regex pattern = new($@"<{Tag}\sgender=(\d+)\sdefinite=(\d+)\sindefinite=(\d+)\splural=(\d+)\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException($"Proper usage: <{Tag} gender=# definite=# indefinite=# plural=# /> where # are any 8-bit integers. Valid example: <{Tag} gender=2 definite=1 indefinite=1 plural=0 />");
        }
        _gender = byte.Parse(m.Groups[1].Value);
        _definite = byte.Parse(m.Groups[2].Value);
        _indefinite = byte.Parse(m.Groups[3].Value);
        _plural = byte.Parse(m.Groups[4].Value);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[ParamSize + 8];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(AppTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(ParamSize), 6);
        bytes[8] = _gender;
        bytes[9] = _definite;
        bytes[10] = _indefinite;
        bytes[11] = _plural;
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<{Tag} gender={_gender} definite={_definite} indefinite={_indefinite} plural={_plural} />";
    }
}