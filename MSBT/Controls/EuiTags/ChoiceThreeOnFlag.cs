using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EuiTags;

internal class ChoiceThreeOnFlag : Control
{
    public const string Tag = "Choice3OnFlag";
    public const ushort TagType = 0x0008;
    private readonly ushort _paramSize;
    private readonly ushort _varType;
    private readonly string _flag0;
    private readonly ushort _choice0;
    private readonly string _flag1;
    private readonly ushort _choice1;
    private readonly string _flag2;
    private readonly ushort _choice2;
    private readonly ushort _defaultIndex;
    private readonly ushort _cancelIndex;

    public ChoiceThreeOnFlag(ref VariableByteQueue queue)
    {
        _paramSize = queue.DequeueU16();
        _varType = queue.DequeueU16();
        (_, _flag0) = queue.DequeueCString();
        _choice0 = queue.DequeueU16();
        (_, _flag1) = queue.DequeueCString();
        _choice1 = queue.DequeueU16();
        (_, _flag2) = queue.DequeueCString();
        _choice2 = queue.DequeueU16();
        _defaultIndex = queue.DequeueU16();
        _cancelIndex = queue.DequeueU16();
    }
    public ChoiceThreeOnFlag(string str)
    {
        Regex pattern = new($@"<{Tag}\stype=\w+(?:\((-?\d+)\))?\sname0='((?:\w|\d)*)'\sresp0=(\d+)\sname1='((?:\w|\d)*)'\sresp1=(\d+)\sname2='((?:\w|\d)*)'\sresp2=(\d+)\sdefault=(\d)\scancel=(\d)\s/>");
        Match m = pattern.Match(str);
        if (!m.Success)
        {
            throw new ArgumentException($@"Proper usage: <{Tag} type=?(#) name0='?' resp0=# name1='?' resp1=# name2='?' resp2=# cancel=# />
                    where all # are 16-bit integers, ? are strings of the applicable purpose, and cancel is the
                    1-based index of the choice that ends the dialogue. Valid examples: 
                    <{Tag} type=Bool(9) name0='Npc_Zora032_Mifa' resp0=34 name1='' resp1=33 name2='' resp2=7 default=0 cancel=3 /> or 
                    <{Tag} type=None(-1) name0='' resp0=16 name1='' resp1=17 name2='' resp2=18 default=1 cancel=3 />");
        }
        _varType = (ushort)short.Parse(m.Groups[1].Value);
        _flag0 = m.Groups[2].Value;
        ushort name0Size = (ushort)(_flag0.Length * 2);
        _choice0 = ushort.Parse(m.Groups[3].ToString());
        _flag1 = m.Groups[4].Value;
        ushort name1Size = (ushort)(_flag1.Length * 2);
        _choice1 = ushort.Parse(m.Groups[5].ToString());
        _flag2 = m.Groups[6].Value;
        ushort name2Size = (ushort)(_flag2.Length * 2);
        _choice2 = ushort.Parse(m.Groups[7].ToString());
        _defaultIndex = ushort.Parse(m.Groups[8].ToString());
        _cancelIndex = ushort.Parse(m.Groups[9].ToString());
        _paramSize = (ushort)(18 + name0Size + name1Size + name2Size);
    }
    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        List<byte> buffer = new(_paramSize + 8);
        buffer.AddRange(converter.GetBytes(ControlTag));
        buffer.AddRange(converter.GetBytes(EuiTag.Group));
        buffer.AddRange(converter.GetBytes(TagType));
        buffer.AddRange(converter.GetBytes(_paramSize));
        buffer.AddRange(converter.GetBytes(_varType));
        buffer.AddRange(converter.GetCString(_flag0));
        buffer.AddRange(converter.GetBytes(_choice0));
        buffer.AddRange(converter.GetCString(_flag1));
        buffer.AddRange(converter.GetBytes(_choice1));
        buffer.AddRange(converter.GetCString(_flag2));
        buffer.AddRange(converter.GetBytes(_choice2));
        buffer.AddRange(converter.GetBytes(_defaultIndex));
        buffer.AddRange(converter.GetBytes(_cancelIndex));
        return buffer.ToArray();
    }
    public override string ToControlString()
    {
        VariableType type = _varType == 0xFFFF ? VariableType.None : _varType < ControlHelpers.VariableTypes.Length ? ControlHelpers.VariableTypes[_varType] : VariableType.Unknown;
        return $"<{Tag} type={type}({(short)_varType}) name0='{_flag0}' resp0={_choice0} name1='{_flag1}' resp1={_choice1} name2='{_flag2}' resp2={_choice2} default={_defaultIndex} cancel={_cancelIndex} />";
    }
}