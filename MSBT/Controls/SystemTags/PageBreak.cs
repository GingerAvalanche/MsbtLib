namespace MsbtLib.Controls.SystemTags;

internal class PageBreak : Control
{
    public const string Tag = "br";
    public const ushort TagType = 0x0004;

    public PageBreak(ref VariableByteQueue queue)
    {
        System.Diagnostics.Debug.WriteLine("Found PageBreak");
    }

    public PageBreak(string str)
    {
        if (str != $"<{Tag} />" && str != $"<{Tag}>")
        {
            throw new ArgumentException("Invalid PageBreak string");
        }
    }

    public override byte[] ToControlSequence(EndiannessConverter converter)
    {
        byte[] bytes = new byte[6];
        bytes.Merge(converter.GetBytes(ControlTag), 0);
        bytes.Merge(converter.GetBytes(SystemTag.Group), 2);
        bytes.Merge(converter.GetBytes(TagType), 4);
        return bytes;
    }
    public override string ToControlString()
    {
        return $"<{Tag} />";
    }
}