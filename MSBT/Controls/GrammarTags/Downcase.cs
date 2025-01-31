namespace MsbtLib.Controls.GrammarTags;

internal class Downcase : Control
{
    public const string Tag = nameof(Downcase);
    public const ushort TagType = 0x0004;

    public Downcase(ref VariableByteQueue queue) { }

    public Downcase(string str)
    {
        if (str != $"<{Tag}>" && str != $"<{Tag} />")
        {
            throw new ArgumentException("Invalid Downcase string.");
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