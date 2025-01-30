using MsbtLib.Controls.ThreeTags;

namespace MsbtLib.Controls;

internal static class ThreeTag
{
    public const ushort Group = 0x0003;
    
    public static Control GetTag(ref VariableByteQueue queue)
    {
        ushort type = queue.DequeueU16();
        return type switch
        {
            Sound.TagType => new Sound(ref queue),
            _ => throw new NotImplementedException($"03 {type} {string.Join(" ", queue.ToArray())}"),
        };
    }
}