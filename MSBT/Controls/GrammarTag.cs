using MsbtLib.Controls.GrammarTags;

namespace MsbtLib.Controls;

internal static class GrammarTag
{
    public const ushort Group = 0x00C9;
    
    public static Control GetTag(ref VariableByteQueue queue)
    {
        ushort type = queue.DequeueU16();
        return type switch
        {
            Info.TagType => new Info(ref queue),
            Definite.TagType => new Definite(ref queue),
            Indefinite.TagType => new Indefinite(ref queue),
            Capitalize.TagType => new Capitalize(ref queue),
            Downcase.TagType => new Downcase(ref queue),
            Gender.TagType => new Gender(ref queue),
            Pluralize.TagType => new Pluralize(ref queue),
            LongVowel.TagType => new LongVowel(ref queue),
            LongVowel2.TagType => new LongVowel2(ref queue),
            9 => throw new NotImplementedException($"201 {type} {string.Join(" ", queue.ToArray())}"),
            10 => throw new NotImplementedException($"201 {type} {string.Join(" ", queue.ToArray())}"),
            _ => throw new NotImplementedException($"201 {type} {string.Join(" ", queue.ToArray())}"),
        };
    }
}