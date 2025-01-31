using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EuiTags;

internal class FiveFlags : Control
{
    public const string Tag = nameof(FiveFlags);
    public const ushort TagType = 0x0009;
    private readonly ushort _paramSize;
    private readonly ushort _flagIdx0;
    private readonly string _flag0;
    private readonly ushort _flagIdx1;
    private readonly string _flag1;
    private readonly ushort _flagIdx2;
    private readonly string _flag2;
    private readonly ushort _flagIdx3;
    private readonly string _flag3;
    private readonly ushort _flagIdx4;
    private readonly string _flag4;
    private readonly ushort _flagVal0;
    private readonly ushort _flagVal1;
    private readonly ushort _flagVal2;
    private readonly ushort _flagVal3;
    private readonly ushort _flagVal4;
    private readonly byte[] _unk;

    public FiveFlags(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        _flagIdx0 = queue.DequeueU16();
        (ushort size0, _flag0) = queue.DequeueCString();
        _flagIdx1 = queue.DequeueU16();
        (ushort size1, _flag1) = queue.DequeueCString();
        _flagIdx2 = queue.DequeueU16();
        (ushort size2, _flag2) = queue.DequeueCString();
        _flagIdx3 = queue.DequeueU16();
        (ushort size3, _flag3) = queue.DequeueCString();
        _flagIdx4 = queue.DequeueU16();
        (ushort size4, _flag4) = queue.DequeueCString();
        _flagVal0 = queue.DequeueU16();
        _flagVal1 = queue.DequeueU16();
        _flagVal2 = queue.DequeueU16();
        _flagVal3 = queue.DequeueU16();
        _flagVal4 = queue.DequeueU16();
        int remaining = _paramSize - (size0 + size1 + size2 + size3 + size4 + 30);
        _unk = new byte[remaining];
        for (int i = 0; i < remaining; ++i)
        {
            _unk[i] = queue.DequeueU8();
        }
    }

    public FiveFlags(string str)
    {
        Regex pattern = new($@"<{Tag}\sidx0=(\d)\sname0='(\w*)'\sval0=(-?\d+)\sidx1=(\d)\sname1='(\w*)'\sval1=(-?\d)\sidx2=(\d)\sname2='(\w*)'\sval2=(-?\d)\sidx3=(\d)\sname3='(\w*)'\sval3=(-?\d)\sidx4=(\d)\sname4='(\w*)'\sval4={{}}\sunk='(.*)'\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException("...I ain't typing up a thing on FiveFlags. I wouldn't be able to format it in a way you could read, anyway.");
        }
        _flagIdx0 = ushort.Parse(m.Groups[1].Value);
        _flag0 = m.Groups[2].Value;
        ushort size0 = (ushort)(_flag0.Length * 2);
        _flagVal0 = ushort.Parse(m.Groups[3].Value);
        _flagIdx1 = ushort.Parse(m.Groups[4].Value);
        _flag1 = m.Groups[5].Value;
        ushort size1 = (ushort)(_flag1.Length * 2);
        _flagVal1 = ushort.Parse(m.Groups[6].Value);
        _flagIdx2 = ushort.Parse(m.Groups[7].Value);
        _flag2 = m.Groups[8].Value;
        ushort size2 = (ushort)(_flag2.Length * 2);
        _flagVal2 = ushort.Parse(m.Groups[9].Value);
        _flagIdx3 = ushort.Parse(m.Groups[10].Value);
        _flag3 = m.Groups[11].Value;
        ushort size3 = (ushort)(_flag3.Length * 2);
        _flagVal3 = ushort.Parse(m.Groups[12].Value);
        _flagIdx4 = ushort.Parse(m.Groups[13].Value);
        _flag4 = m.Groups[14].Value;
        ushort size4 = (ushort)(_flag4.Length * 2);
        _flagVal4 = ushort.Parse(m.Groups[15].Value);
        _unk = m.Groups[16].Value.Split(' ').Select(byte.Parse).ToArray();
        _paramSize = (ushort)(_unk.Length + size0 + size1 + size2 + size3 + size4 + 30);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(_paramSize + 8);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(EuiTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_paramSize));
        buffer.AddRange(converter.GetBytes(_flagIdx0));
        buffer.AddRange(converter.GetCString(_flag0));
        buffer.AddRange(converter.GetBytes(_flagIdx1));
        buffer.AddRange(converter.GetCString(_flag1));
        buffer.AddRange(converter.GetBytes(_flagIdx2));
        buffer.AddRange(converter.GetCString(_flag2));
        buffer.AddRange(converter.GetBytes(_flagIdx3));
        buffer.AddRange(converter.GetCString(_flag3));
        buffer.AddRange(converter.GetBytes(_flagIdx4));
        buffer.AddRange(converter.GetCString(_flag4));
        buffer.AddRange(converter.GetBytes(_flagVal0));
        buffer.AddRange(converter.GetBytes(_flagVal1));
        buffer.AddRange(converter.GetBytes(_flagVal2));
        buffer.AddRange(converter.GetBytes(_flagVal3));
        buffer.AddRange(converter.GetBytes(_flagVal4));
        buffer.AddRange(_unk);
        return buffer.ToArray();
    }

    public override string ToControlString()
    {
        return $"<{Tag} idx0={_flagIdx0} name0={_flag0} val0={(short)_flagVal0} idx1={_flagIdx1} name1={_flag1} val1={(short)_flagVal1} idx2={_flagIdx2} name2={_flag2} val2={(short)_flagVal2} idx3={_flagIdx3} name3={_flag3} val3={(short)_flagVal3} idx4={_flagIdx4} name4={_flag4} val4={(short)_flagVal4} unk='{string.Join(' ', _unk)}' />";
    }
}