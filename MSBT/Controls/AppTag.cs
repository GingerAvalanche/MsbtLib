using MsbtLib.Controls.App;

namespace MsbtLib.Controls;

internal static class AppTag
{
    public const ushort Group = 0x0002;

    public static Control GetTag(ref VariableByteQueue queue)
    {
        // All group 5 are pause length, type is the parameter
        return new Variable(ref queue);
    }
}