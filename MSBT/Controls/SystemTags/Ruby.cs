using System.Text.RegularExpressions;

namespace MsbtLib.Controls.SystemTags;

internal class Ruby : Control
{
    public const string Tag = nameof(Ruby);
    public const ushort TagType = 0x0000;
    private readonly ushort _field0;
    private readonly ushort _field1;
    private readonly ushort _field2;

    public Ruby(ref VariableByteQueue queue)
    {
        _field0 = queue.DequeueU16();
        _field1 = queue.DequeueU16();
        _field2 = queue.DequeueU16();
    }

    public Ruby(string str)
    {
        Regex pattern = new($@"<{Tag}\s0=(\d+)\s1=(\d+)\s2=(\d+)\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException($"Proper usage: <{Tag} 0=# 1=# 2=# where # are 16-bit integers. />");
        }
        _field0 = ushort.Parse(m.Groups[1].ToString());
        _field1 = ushort.Parse(m.Groups[2].ToString());
        _field2 = ushort.Parse(m.Groups[3].ToString());
    }
    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(12);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(SystemTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_field0));
        buffer.AddRange(converter.GetBytes(_field1));
        buffer.AddRange(converter.GetBytes(_field2));
        return buffer.ToArray();
    }
    public override string ToControlString()
    {
        return $"<{Tag} 0={_field0} 1={_field1} 2={_field2} />";
    }
}