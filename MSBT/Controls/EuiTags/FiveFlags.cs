using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EuiTags;

internal class FiveFlags : Control
{
    public const string Tag = nameof(FiveFlags);
    public const ushort TagType = 0x0009;
    private readonly ushort _paramSize;
    private readonly ushort _flagIdx0;
    private readonly ushort _flagSize0;
    private readonly string _flagName0;
    private readonly ushort _flagIdx1;
    private readonly ushort _flagSize1;
    private readonly string _flagName1;
    private readonly ushort _flagIdx2;
    private readonly ushort _flagSize2;
    private readonly string _flagName2;
    private readonly ushort _flagIdx3;
    private readonly ushort _flagSize3;
    private readonly string _flagName3;
    private readonly ushort _flagIdx4;
    private readonly ushort _flagSize4;
    private readonly string _flagName4;
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
        _flagSize0 = queue.DequeueU16();
        StringBuilder b = new();
        for (int i = 0; i < _flagSize0 / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName0 = b.ToString();
        _flagIdx1 = queue.DequeueU16();
        _flagSize1 = queue.DequeueU16();
        b.Clear();
        for (int i = 0; i < _flagSize1 / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName1 = b.ToString();
        _flagIdx2 = queue.DequeueU16();
        _flagSize2 = queue.DequeueU16();
        b.Clear();
        for (int i = 0; i < _flagSize2 / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName2 = b.ToString();
        _flagIdx3 = queue.DequeueU16();
        _flagSize3 = queue.DequeueU16();
        b.Clear();
        for (int i = 0; i < _flagSize3 / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName3 = b.ToString();
        _flagIdx4 = queue.DequeueU16();
        _flagSize4 = queue.DequeueU16();
        b.Clear();
        for (int i = 0; i < _flagSize4 / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName4 = b.ToString();
        _flagVal0 = queue.DequeueU16();
        _flagVal1 = queue.DequeueU16();
        _flagVal2 = queue.DequeueU16();
        _flagVal3 = queue.DequeueU16();
        _flagVal4 = queue.DequeueU16();
        int remaining = _paramSize - (_flagSize0 + _flagSize1 + _flagSize2 + _flagSize3 + _flagSize4 + 30);
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
        _flagName0 = m.Groups[2].Value;
        _flagSize0 = (ushort)(_flagName0.Length * 2);
        _flagVal0 = ushort.Parse(m.Groups[3].Value);
        _flagIdx1 = ushort.Parse(m.Groups[4].Value);
        _flagName1 = m.Groups[5].Value;
        _flagSize1 = (ushort)(_flagName1.Length * 2);
        _flagVal1 = ushort.Parse(m.Groups[6].Value);
        _flagIdx2 = ushort.Parse(m.Groups[7].Value);
        _flagName2 = m.Groups[8].Value;
        _flagSize2 = (ushort)(_flagName2.Length * 2);
        _flagVal2 = ushort.Parse(m.Groups[9].Value);
        _flagIdx3 = ushort.Parse(m.Groups[10].Value);
        _flagName3 = m.Groups[11].Value;
        _flagSize3 = (ushort)(_flagName3.Length * 2);
        _flagVal3 = ushort.Parse(m.Groups[12].Value);
        _flagIdx4 = ushort.Parse(m.Groups[13].Value);
        _flagName4 = m.Groups[14].Value;
        _flagSize4 = (ushort)(_flagName4.Length * 2);
        _flagVal4 = ushort.Parse(m.Groups[15].Value);
        _unk = m.Groups[16].Value.Split(' ').Select(byte.Parse).ToArray();
        _paramSize = (ushort)(_unk.Length + _flagSize0 + _flagSize1 + _flagSize2 + _flagSize3 + _flagSize4 + 30);
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(_paramSize + 8);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(EuiTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_paramSize));
        buffer.AddRange(converter.GetBytes(_flagIdx0));
        buffer.AddRange(converter.GetBytes(_flagSize0));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName0));
        buffer.AddRange(converter.GetBytes(_flagIdx1));
        buffer.AddRange(converter.GetBytes(_flagSize1));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName1));
        buffer.AddRange(converter.GetBytes(_flagIdx2));
        buffer.AddRange(converter.GetBytes(_flagSize2));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName2));
        buffer.AddRange(converter.GetBytes(_flagIdx3));
        buffer.AddRange(converter.GetBytes(_flagSize3));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName3));
        buffer.AddRange(converter.GetBytes(_flagIdx4));
        buffer.AddRange(converter.GetBytes(_flagSize4));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName4));
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
        return $"<{Tag} idx0={_flagIdx0} name0={_flagName0} val0={(short)_flagVal0} idx1={_flagIdx1} name1={_flagName1} val1={(short)_flagVal1} idx2={_flagIdx2} name2={_flagName2} val2={(short)_flagVal2} idx3={_flagIdx3} name3={_flagName3} val3={(short)_flagVal3} idx4={_flagIdx4} name4={_flagName4} val4={(short)_flagVal4} unk='{string.Join(' ', _unk)}' />";
    }
}