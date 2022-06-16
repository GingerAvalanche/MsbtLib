using MsbtLib.Controls;
using System.Text.RegularExpressions;

namespace MsbtLib
{
    abstract internal class Control
    {
        public const ushort control_tag = 0x000E;
        public static Control GetControl(ref Queue<byte> queue, EndiannessConverter converter)
        {
            ushort tag_group = converter.Convert(queue.DequeueUInt16());
            ushort tag_type = converter.Convert(queue.DequeueUInt16());
            ushort param_size = converter.Convert(queue.DequeueUInt16());
            List<ushort> parameters = new() { param_size };
            foreach (var _ in Enumerable.Range(0, param_size / 2))
            {
                parameters.Add(converter.Convert(queue.DequeueUInt16()));
            }
            switch (tag_group)
            {
                case 0:
                    switch (tag_type)
                    {
                        case 1:
                            return new Font(parameters);
                        case 2:
                            return new TextSize(parameters);
                        case 3:
                            return new SetColor(parameters);
                    }
                    break;
                case 1:
                    switch (tag_type)
                    {
                        case 0:
                            return new PauseFrames(parameters);
                        case 3:
                            return new AutoAdvance(parameters);
                        case 4:
                            return new ChoiceTwo(parameters);
                        case 5:
                            return new ChoiceThree(parameters);
                        case 6:
                            return new ChoiceFour(parameters);
                        case 7:
                            return new Icon(parameters);
                        case 10:
                            return new ChoiceOne(parameters);
                    }
                    break;
                case 2:
                    if (param_size > 0)
                    {
                        return new Variable(tag_type, parameters); // until we figure out what a Variable's tag_type is for, I'm scorning Nintendo some more
                    }
                    break;
                case 3:
                    switch (tag_type)
                    {
                        case 1:
                            parameters[1] = converter.Convert(parameters[1]); // this is actually 2 byte params, so we need to re-reverse them if they've been reversed
                            return new Sound(parameters);
                    }
                    break;
                case 4:
                    switch (tag_type)
                    {
                        case 1:
                            parameters[1] = converter.Convert(parameters[1]); // this is actually 2 byte params, so we need to re-reverse them if they've been reversed
                            return new Sound2(parameters);
                        case 2:
                            return new Animation(parameters);
                    }
                    break;
                case 5:
                    parameters.Add(tag_type); // yes, the tag_type is actually the parameter for PauseLengths. Thanks, Nintendo
                    return new PauseLength(parameters);
            }
            return new RawControl(tag_group, tag_type, parameters);
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
                "textsize" => new TextSize(s),
                "color" => new SetColor(s),
                "pauseframes" => new PauseFrames(s),
                "auto_advance" => new AutoAdvance(s),
                "choice2" => new ChoiceTwo(s),
                "choice3" => new ChoiceThree(s),
                "choice4" => new ChoiceFour(s),
                "icon" => new Icon(s),
                "choice1" => new ChoiceOne(s),
                "variable" => new Variable(s),
                "sound" => new Sound(s),
                "sound2" => new Sound2(s),
                "animation" => new Animation(s),
                "pauselength" => new PauseLength(s),
                "raw" => new RawControl(s),
                _ => throw new ArgumentException($"Invalid control string: {s}")
            };
        }
        public abstract byte[] ToControlSequence(EndiannessConverter converter);
        public abstract string ToControlString();
    }
}
