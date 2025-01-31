using MsbtLib.Controls.GrammarTags;

namespace MsbtLib.Controls;

internal class GrammarTag
{
    public const ushort Group = 0x00C9;
    
    public static Control GetTag(ref VariableByteQueue queue)
    {
        ushort type = queue.DequeueU16();
        return type switch
        {
            Info.TagType => new Info(ref queue),
            1 => throw new NotImplementedException("201-1"),
            2 => throw new NotImplementedException("201-2"),
            Capitalize.TagType => new Capitalize(ref queue),
            Downcase.TagType => new Downcase(ref queue),
            5 => throw new NotImplementedException("201-5"),
            6 => throw new NotImplementedException("201-6"),
            7 => throw new NotImplementedException("201-7"),
            8 => throw new NotImplementedException("201-8"),
            9 => throw new NotImplementedException("201-9"),
            10 => throw new NotImplementedException("201-10"),
            _ => throw new NotImplementedException($"201 {type} {string.Join(" ", queue.ToArray())}"),
        };
    }
}