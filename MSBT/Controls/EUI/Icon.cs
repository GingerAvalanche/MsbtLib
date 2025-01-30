using System.Text.RegularExpressions;

namespace MsbtLib.Controls.EUI
{
    enum IconType
    {
        // ReSharper disable once InconsistentNaming
        ZL,
        L,
        R,
        Y,
        X,
        A,
        B,
        Plus,
        Minus,
        DPadDown,
        DPadLeft,
        DPadRight,
        DPadUp,
        RStickHorizontal,
        RStickPress,
        RStickVertical,
        LStickBack,
        LStickForward,
        LStickLeft,
        LStickPress,
        LStickRight,
        Gamepad,
        LeftArrow,
        RightArrow,
        UpArrow,
        Unknown
    }
    internal class Icon : Control
    {
        private static readonly IconType[] IconTypes =
        [
            IconType.LStickForward,
            IconType.LStickBack,
            IconType.LStickLeft,
            IconType.LStickRight,
            IconType.RStickVertical,
            IconType.RStickHorizontal,
            IconType.DPadUp,
            IconType.DPadDown,
            IconType.DPadLeft,
            IconType.DPadRight,
            IconType.A,
            IconType.A,
            IconType.X,
            IconType.Y,
            IconType.ZL,
            IconType.ZL,
            IconType.Unknown,
            IconType.B,
            IconType.Unknown,
            IconType.Unknown,
            IconType.L,
            IconType.R,
            IconType.Unknown,
            IconType.Plus,
            IconType.Minus,
            IconType.RightArrow,
            IconType.LeftArrow,
            IconType.UpArrow,
            IconType.Unknown,
            IconType.Unknown,
            IconType.Unknown,
            IconType.Unknown,
            IconType.Unknown,
            IconType.LStickPress,
            IconType.RStickPress,
            IconType.Unknown,
            IconType.Gamepad,
            IconType.X,
            IconType.X
        ];

        public const string Tag = nameof(Icon);
        public const ushort TagType = 0x0007;
        private const ushort ParamSize = 2;
        private readonly byte _num;
        public Icon(ref VariableByteQueue queue)
        {
            if (queue.DequeueU16() != ParamSize) throw new InvalidDataException("Icon parameter size mismatch");
            _num = queue.DequeueU8();
            if (queue.DequeueU8() != 0xCD) throw new InvalidDataException("Icon ending byte not 0xCD");
        }
        public Icon(string str)
        {
            Regex pattern = new($@"<{Tag}=(\w+)(?:\((-?\d+)\))?\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new Exception($"Proper usage: <{Tag}=?(#) /> where ? is an icon name and (#) is an optional key for that icon. Valid examples: <{Tag}=DPadDown /> or <{Tag}=X(38) />");
            }
            string parsedKey = m.Groups[1].ToString();
            IconType icon = (IconType)Enum.Parse(typeof(IconType), parsedKey);
            if (icon is IconType.A or IconType.X or IconType.ZL or IconType.Unknown)
            {
                string byteStr = m.Groups[2].ToString();
                if (String.IsNullOrEmpty(byteStr))
                {
                    throw new Exception($"IconType.{icon} requires a specified key, for example A(10) or ZL(15)");
                }
                int parsedNum = int.Parse(byteStr);
                if (parsedNum is < 0 or > 255)
                {
                    throw new Exception($"Icon keys must be 1 unsigned byte. i.e. Their value must be >=0 and <=255.");
                }
                if (IconTypes[parsedNum] == icon || parsedNum >= IconTypes.Length)
                {
                    _num = (byte)parsedNum;
                }
                else
                {
                    throw new Exception($"IconType.{icon} had an invalid specified key. Invalid key: {parsedNum}");
                }
            }
            else
            {
                _num = (byte)Array.IndexOf(IconTypes, icon);
            }
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[ParamSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(EuiTag.Group), 2);
            bytes.Merge(converter.GetBytes(TagType), 4);
            bytes.Merge(converter.GetBytes(ParamSize), 6);
            bytes[8] = _num;
            bytes[9] = 0xCD;
            return bytes;
        }
        public override string ToControlString()
        {
            IconType icon = _num < IconTypes.Length ? IconTypes[_num] : IconType.Unknown;
            string numStr = $"";
            if (icon is IconType.A or IconType.X or IconType.ZL or IconType.Unknown)
            {
                numStr = $"({_num})";
            }
            return $"<{Tag}={icon}{numStr} />";
        }
    }
}
