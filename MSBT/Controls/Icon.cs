using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    enum IconType
    {
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
        private static readonly IconType[] iconTypes = new IconType[]
        {
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
            IconType.X,
        };
        public const ushort tag_group = 0x0001;
        public const ushort tag_type = 0x0007;
        private ushort param_size;
        private byte num;
        public Icon(List<ushort> parameters)
        {
            param_size = parameters[0];
            byte[] param_bytes = BitConverter.GetBytes(parameters[1]);
            num = param_bytes[0] == 0xCD ? param_bytes[1] : param_bytes[0];
        }
        public Icon(string str)
        {
            Regex pattern = new(@"<icon=(\w+)(?:\((-?\d+)\))?\s/>");
            Match m = pattern.Match(str);
            if (!m.Success)
            {
                throw new Exception("Proper usage: <icon=?(#) /> where ? is an icon name and (#) is an optional key for that icon. Valid examples: <icon=DPadDown /> or <icon=X(38) />");
            }
            string parsedKey = m.Groups[1].ToString();
            IconType icon = (IconType)Enum.Parse(typeof(IconType), parsedKey);
            if (icon == IconType.A || icon == IconType.X || icon == IconType.ZL || icon == IconType.Unknown)
            {
                string byteStr = m.Groups[2].ToString();
                if (String.IsNullOrEmpty(byteStr))
                {
                    throw new Exception($"IconType.{icon} requires a specified key, for example A(10) or ZL(15)");
                }
                int parsedNum = int.Parse(byteStr);
                if (parsedNum < 0 || parsedNum > 255)
                {
                    throw new Exception($"Icon keys must be 1 unsigned byte. i.e. Their value must be >=0 and <=255.");
                }
                if (iconTypes[parsedNum] == icon || parsedNum >= iconTypes.Length)
                {
                    num = (byte)parsedNum;
                }
                else
                {
                    throw new Exception($"IconType.{icon} had an invalid specified key. Invalid key: {parsedNum}");
                }
            }
            else
            {
                num = (byte)Array.IndexOf(iconTypes, icon);
            }
            param_size = 2;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes[8] = num;
            bytes[9] = 0xCD;
            return bytes;
        }
        public override string ToControlString()
        {
            IconType icon = num < iconTypes.Length ? iconTypes[num] : IconType.Unknown;
            string numStr = $"";
            if (icon == IconType.A || icon == IconType.X || icon == IconType.ZL || icon == IconType.Unknown)
            {
                numStr = $"({num})";
            }
            return $"<icon={icon}{numStr} />";
        }
    }
}
