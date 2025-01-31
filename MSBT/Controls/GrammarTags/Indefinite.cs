namespace MsbtLib.Controls.GrammarTags;

internal class Indefinite : Control
{
    public const string Tag = nameof(Indefinite);
    public const ushort TagType = 0x0002;
    private const ushort ParamSize = 0;

    public Indefinite(ref VariableByteQueue queue)
    {
        if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Indefinite parameter size mismatch");
    }

    public Indefinite(string str)
    {
        if (str != $"<{Tag}>" && str != $"<{Tag} />")
        {
            throw new ArgumentException("Invalid Indefinite string.");
        }
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[6];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(GrammarTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(ParamSize), 6);
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<{Tag} />";
    }
}