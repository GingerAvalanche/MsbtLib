namespace MsbtLib.Controls.EuiTags;

internal class NoScroll : Control
{
    public const string Tag = nameof(NoScroll);
    public const ushort TagType = 0x0002;
    private const ushort ParamSize = 0;
    public NoScroll(ref VariableByteQueue queue)
    {
        if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("TextSpeed parameter size mismatch");
    }

    public NoScroll(string str)
    {
        if (str != $"<{Tag}>" && str != $"<{Tag} />") throw new ArgumentException("NoScroll cannot have parameters");
    }
    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[ParamSize + 8];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(EuiTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(ParamSize), 6);
        return bytes;
    }
    public override string ToControlString()
    {
        return $"<{Tag} />";
    }
}