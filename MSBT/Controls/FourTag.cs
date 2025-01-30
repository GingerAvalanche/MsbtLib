using MsbtLib.Controls.Four;

namespace MsbtLib.Controls;

internal static class FourTag
{
    public const ushort Group = 0x0004;
    
    public static Control GetTag(ref VariableByteQueue queue)
    {
        ushort type = queue.DequeueU16();
        return type switch
        {
            FourZero.TagType => new FourZero(ref queue),
            Sound2.TagType => new Sound2(ref queue),
            Animation.TagType => new Animation(ref queue),
            FourThree.TagType => new FourThree(ref queue),
            _ => throw new NotImplementedException($"04 {type} {string.Join(" ", queue.ToArray())}"),
        };
    }
}