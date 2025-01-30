using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EuiTags;

internal class ChoiceThreeOnFlag : Control
{
    public const string Tag = "Choice3OnFlag";
    public const ushort TagType = 0x0008;
    private readonly ushort _paramSize;
    private readonly ushort _varType;
    private readonly ushort _name0Size;
    private readonly ushort _choice0;
    private readonly ushort _name1Size;
    private readonly ushort _choice1;
    private readonly ushort _name2Size;
    private readonly ushort _choice2;
    private readonly ushort _defaultIndex;
    private readonly ushort _cancelIndex;
    private readonly string _flagName0;
    private readonly string _flagName1;
    private readonly string _flagName2;

    public ChoiceThreeOnFlag(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        _varType = queue.DequeueU16();
        _name0Size = queue.DequeueU16();
        StringBuilder b = new();
        for (int i = 0; i < _name0Size / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName0 = b.ToString();
        _choice0 = queue.DequeueU16();
        _name1Size = queue.DequeueU16();
        b.Clear();
        for (int i = 0; i < _name1Size / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName1 = b.ToString();
        _choice1 = queue.DequeueU16();
        _name2Size = queue.DequeueU16();
        b.Clear();
        for (int i = 0; i < _name2Size / 2; ++i)
        {
            b.Append(Convert.ToChar(queue.DequeueU16()));
        }
        _flagName2 = b.ToString();
        _choice2 = queue.DequeueU16();
        _defaultIndex = queue.DequeueU16();
        _cancelIndex = queue.DequeueU16();
    }
    public ChoiceThreeOnFlag(string str)
    {
        Regex pattern = new($@"<{Tag}\stype=\w+(?:\((-?\d+)\))?\sname0='(\w*)'\sresp0=(\d+)\sname1='(\w*)'\sresp1=(\d+)\sname2='(\w*)'\sresp2=(\d+)\sdefault=(\d)\scancel=(\d)\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException($@"Proper usage: <{Tag} type=?(#) name='?' 0=# 1=# 2=# cancel=# />
                    where all # are 16-bit integers, ? are strings of the applicable purpose, and cancel is the
                    1-based index of the choice that ends the dialogue. Valid examples: 
                    <{Tag} type=Bool(9) name='Npc_Zora032_Mifa' 0=34 1=33 2=7 default=0 cancel=3 /> or 
                    <{Tag} type=None(-1) name='' 0=16 1=17 2=18 default=1 cancel=3 />");
        }
        _varType = (ushort)short.Parse(m.Groups[1].Value);
        _flagName0 = m.Groups[2].Value;
        _name0Size = (ushort)(_flagName0.Length * 2);
        _choice0 = ushort.Parse(m.Groups[3].ToString());
        _flagName1 = m.Groups[4].Value;
        _name1Size = (ushort)(_flagName1.Length * 2);
        _choice1 = ushort.Parse(m.Groups[5].ToString());
        _flagName2 = m.Groups[6].Value;
        _name2Size = (ushort)(_flagName2.Length * 2);
        _choice2 = ushort.Parse(m.Groups[7].ToString());
        _defaultIndex = ushort.Parse(m.Groups[8].ToString());
        _cancelIndex = ushort.Parse(m.Groups[9].ToString());
        _paramSize = (ushort)(18 + _name0Size + _name1Size + _name2Size);
    }
    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(_paramSize + 8);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(EuiTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_paramSize));
        buffer.AddRange(converter.GetBytes(_varType));
        buffer.AddRange(converter.GetBytes(_name0Size));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName0));
        buffer.AddRange(converter.GetBytes(_choice0));
        buffer.AddRange(converter.GetBytes(_name1Size));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName1));
        buffer.AddRange(converter.GetBytes(_choice1));
        buffer.AddRange(converter.GetBytes(_name2Size));
        buffer.AddRange(Encoding.Unicode.GetBytes(_flagName2));
        buffer.AddRange(converter.GetBytes(_choice2));
        buffer.AddRange(converter.GetBytes(_defaultIndex));
        buffer.AddRange(converter.GetBytes(_cancelIndex));
        return buffer.ToArray();
    }
    public override string ToControlString()
    {
        VariableType type = _varType == 0xFFFF ? VariableType.None : _varType < ControlHelpers.VariableTypes.Length ? ControlHelpers.VariableTypes[_varType] : VariableType.Unknown;
        return $"<{Tag} type={type}({(short)_varType}) name0={_flagName0} resp0={_choice0} name1={_flagName1} resp1={_choice1} name2={_flagName2} resp2={_choice2} default={_defaultIndex} cancel={_cancelIndex} />";
    }
}