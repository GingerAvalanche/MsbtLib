using MsbtLib.Controls;
using System.Text.RegularExpressions;
using MsbtLib.Controls.AppTags;
using MsbtLib.Controls.EuiTags;
using MsbtLib.Controls.FiveTags;
using MsbtLib.Controls.FourTags;
using MsbtLib.Controls.GrammarTags;
using MsbtLib.Controls.SystemTags;
using MsbtLib.Controls.ThreeTags;

namespace MsbtLib
{
    internal abstract class Control
    {
        protected const ushort ControlTag = 0x000E;
        public static Control GetControl(ref VariableByteQueue queue)
        {
            ushort tagGroup = queue.DequeueU16();
            switch (tagGroup)
            {
                case SystemTag.Group:
                    return SystemTag.GetTag(ref queue);
                case EuiTag.Group:
                    return EuiTag.GetTag(ref queue);
                case AppTag.Group:
                    return AppTag.GetTag(ref queue);
                case ThreeTag.Group:
                    return ThreeTag.GetTag(ref queue);
                case FourTag.Group:
                    return FourTag.GetTag(ref queue);
                case FiveTag.Group:
                    return FiveTag.GetTag(ref queue);
                case GrammarTag.Group:
                    return GrammarTag.GetTag(ref queue);
            }
            return new RawControl(tagGroup, ref queue);
        }
        public static Control GetControl(string s)
        {
            Regex pattern = new(@"</?(\w*)", RegexOptions.ECMAScript);
            Match m = pattern.Match(s);
            if (!m.Success)
            {
                throw new ArgumentException($"Invalid control string: {s}");
            }
            string type = m.Groups[1].ToString();
            return type switch
            {
                Ruby.Tag => new Ruby(s),
                Font.Tag => new Font(s),
                FontSize.Tag => new FontSize(s),
                FontColor.Tag => new FontColor(s),
                PageBreak.Tag => new PageBreak(s),
                Delay.Tag => new Delay(s),
                TextSpeed.Tag => new TextSpeed(s),
                NoScroll.Tag => new NoScroll(s),
                AutoAdvance.Tag => new AutoAdvance(s),
                ChoiceTwo.Tag => new ChoiceTwo(s),
                ChoiceThree.Tag => new ChoiceThree(s),
                ChoiceFour.Tag => new ChoiceFour(s),
                Icon.Tag => new Icon(s),
                ChoiceThreeOnFlag.Tag => new ChoiceThreeOnFlag(s),
                FiveFlags.Tag => new FiveFlags(s),
                ChoiceOne.Tag => new ChoiceOne(s),
                Variable.Tag => new Variable(s),
                Sound.Tag => new Sound(s),
                FourZero.Tag => new FourZero(s),
                Sound2.Tag => new Sound2(s),
                Animation.Tag => new Animation(s),
                FourThree.Tag => new FourThree(s),
                PauseLength.Tag => new PauseLength(s),
                Info.Tag => new Info(s),
                RawControl.Tag => new RawControl(s),
                _ => throw new ArgumentException($"Invalid control string: {s}")
            };
        }
        public abstract byte[] ToControlSequence(EndiannessConverter converter);
        public abstract string ToControlString();
    }
}
