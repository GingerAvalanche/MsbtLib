using MsbtLib.Controls.FiveTags;

namespace MsbtLib.Controls;

internal static class FiveTag
{
    public const ushort Group = 0x0005;

    public static Control GetTag(ref VariableByteQueue queue)
    {
        // All group 5 are pause length, type is the parameter
        return new PauseLength(ref queue);
    }
}
