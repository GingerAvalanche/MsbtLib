namespace MsbtLib.Controls;

enum VariableType : short
{
    None = -1,
    Int,
    Float,
    String32,
    String64,
    String256,
    Bool,
    Empty,
    Unknown
}

internal static class ControlHelpers
{
    public static readonly VariableType[] VariableTypes =
    [
        VariableType.Bool, // 0
        VariableType.Empty, // 1
        VariableType.Int, // 2
        VariableType.Empty, // 3
        VariableType.Bool, // 4
        VariableType.Bool, // 5
        VariableType.Empty, // 6
        VariableType.Bool, // 7
        VariableType.Bool, // 8
        VariableType.Bool, // 9
        VariableType.Empty, // 10
        VariableType.String64, // 11
        VariableType.Empty, // 12
        VariableType.Empty, // 13
        VariableType.Int, // 14
        VariableType.Int, // 15
        VariableType.Int, // 16
        VariableType.Int, // 17
        VariableType.Int, // 18
        VariableType.Int, // 19
        VariableType.Unknown, // 20
        VariableType.Bool, // 21
        VariableType.Unknown, // 22
        VariableType.Unknown, // 23
        VariableType.Unknown, // 24
        VariableType.Bool, // 25
        VariableType.Unknown, // 26
        VariableType.Unknown, // 27
        VariableType.Unknown, // 28
        VariableType.Unknown, // 29
        VariableType.Unknown, // 30
        VariableType.Unknown, // 31
        VariableType.Unknown, // 32
        VariableType.Unknown, // 33
        VariableType.Bool, // 34
    ];
}