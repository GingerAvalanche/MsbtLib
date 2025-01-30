using MsbtLib.Controls.System;

namespace MsbtLib.Controls;

internal static class SystemTag
{
    public const ushort Group = 0x0000;
    
    public static Control GetTag(ref VariableByteQueue queue)
    {
        ushort type = queue.DequeueU16();
        return type switch
        {
            Ruby.TagType => new Ruby(ref queue),
            Font.TagType => new Font(ref queue),
            FontSize.TagType => new FontSize(ref queue),
            FontColor.TagType => new FontColor(ref queue),
            PageBreak.TagType => new PageBreak(),
            _ => throw new NotImplementedException($"00 {type} {string.Join(" ", queue.ToArray())}"),
        };
    }
}