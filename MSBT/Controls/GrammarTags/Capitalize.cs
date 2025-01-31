namespace MsbtLib.Controls.GrammarTags;

internal class Capitalize : Control
{
    public const string Tag = nameof(Capitalize);
    public const ushort TagType = 0x0003;

    public Capitalize(ref VariableByteQueue queue) { }

    public Capitalize(string str)
    {
        if (str != $"<{Tag}>" && str != $"<{Tag} />")
        {
            throw new ArgumentException("Invalid Capitalize string.");
        }
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[6];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(GrammarTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<{Tag} />";
    }
}