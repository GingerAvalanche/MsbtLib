namespace MsbtLib.Controls.GrammarTags;

internal class Definite : Control
{
    public const string Tag = nameof(Definite);
    public const ushort TagType = 0x0001;
    private const ushort ParamSize = 0;

    public Definite(ref VariableByteQueue queue)
    {
        if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Definite parameter size mismatch");
    }

    public Definite(string str)
    {
        if (str != $"<{Tag}>" && str != $"<{Tag} />")
        {
            throw new ArgumentException("Invalid Definite string.");
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