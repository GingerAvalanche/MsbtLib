using MsbtLib.Controls.EUI;

namespace MsbtLib.Controls;

internal static class EuiTag
{
    public const ushort Group = 0x0001;
    
    public static Control GetTag(ref VariableByteQueue queue)
    {
        ushort type = queue.DequeueU16();
        return type switch
        {
            Delay.TagType => new Delay(ref queue),
            TextSpeed.TagType => new TextSpeed(ref queue),
            NoScroll.TagType => new NoScroll(ref queue),
            AutoAdvance.TagType => new AutoAdvance(ref queue),
            ChoiceTwo.TagType => new ChoiceTwo(ref queue),
            ChoiceThree.TagType => new ChoiceThree(ref queue),
            ChoiceFour.TagType => new ChoiceFour(ref queue),
            Icon.TagType => new Icon(ref queue),
            ChoiceThreeOnFlag.TagType => new ChoiceThreeOnFlag(ref queue),
            FiveFlags.TagType => new FiveFlags(ref queue),
            ChoiceOne.TagType => new ChoiceOne(ref queue),
            _ => throw new NotImplementedException($"01 {type} {string.Join(" ", queue.ToArray())}"),
        };
    }
}
