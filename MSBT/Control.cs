using MsbtLib.Controls;
using System.Text.RegularExpressions;
using MsbtLib.Controls.App;
using MsbtLib.Controls.EUI;
using MsbtLib.Controls.Five;
using MsbtLib.Controls.Four;
using MsbtLib.Controls.System;
using MsbtLib.Controls.Three;

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
                "font" => new Font(s),
                "textsize" => new FontSize(s),
                "color" => new FontColor(s),
                "pauseframes" => new Delay(s),
                "auto_advance" => new AutoAdvance(s),
                "choice2" => new ChoiceTwo(s),
                "choice3" => new ChoiceThree(s),
                "choice4" => new ChoiceFour(s),
                "icon" => new Icon(s),
                "choice3onflag" => new ChoiceThreeOnFlag(s),
                "fiveflags" => new FiveFlags(s),
                "choice1" => new ChoiceOne(s),
                "variable" => new Variable(s),
                "sound" => new Sound(s),
                "fourzero" => new FourZero(s),
                "sound2" => new Sound2(s),
                "animation" => new Animation(s),
                "fourthree" => new FourThree(s),
                "pauselength" => new PauseLength(s),
                "raw" => new RawControl(s),
                _ => throw new ArgumentException($"Invalid control string: {s}")
            };
        }
        public abstract byte[] ToControlSequence(EndiannessConverter converter);
        public abstract string ToControlString();
    }
}
