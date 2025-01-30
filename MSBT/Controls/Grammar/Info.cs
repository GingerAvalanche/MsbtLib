namespace MsbtLib.Controls.Grammar;

internal class Info : Control
{
    public const ushort TagType = 0x0000;
    private const ushort ParamSize = 0x0004;
    private readonly byte _gender;
    private readonly byte _definite;
    private readonly byte _indefinite;
    private readonly byte _plural;

    public Info(ref VariableByteQueue queue)
    {
        _gender = queue.DequeueU8();
        _definite = queue.DequeueU8();
        _indefinite = queue.DequeueU8();
        _plural = queue.DequeueU8();
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        
        byte[] bytes = new byte[ParamSize + 8];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(AppTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        bytes.Merge(converter.GetBytes(ParamSize), 6);
        bytes[8] = _gender;
        bytes[9] = _definite;
        bytes[10] = _indefinite;
        bytes[11] = _plural;
        return bytes;
    }

    public override string ToControlString()
    {
        return $"<info gender={_gender} definite={_definite} indefinite={_indefinite} plural={_plural} />";
    }
}