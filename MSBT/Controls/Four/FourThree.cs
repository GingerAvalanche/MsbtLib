namespace MsbtLib.Controls.Four;

internal class FourThree : Control
{
    public const string Tag = nameof(FourThree);
    public const ushort TagType = 0x0003;
    private const ushort ParamSize = 0;

    public FourThree(ref VariableByteQueue queue)
    {
        if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("FourTag paramSize not 0");
    }
    public FourThree(string str) { }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[8];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(FourTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(ParamSize), 6);
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<{Tag} />";
    }
}
