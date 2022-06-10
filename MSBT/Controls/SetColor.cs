namespace MsbtLib.Controls
{
    enum Color
    {
        Red,
        LightGreen1,
        Blue,
        Grey,
        LightGreen4,
        Orange,
        LightGrey,
        Reset = 0xFFFF
    }
    internal class SetColor : Control
    {
        public Color Color { get; set; }
        public SetColor(ushort col)
        {
            Color = (Color)col;
        }
        public SetColor(string str)
        {
            if (str == "</color>")
            {
                Color = Color.Reset;
            }
            else
            {
                string parsed = str.Replace("<color=", "").Replace(">", "");
                Color = parsed switch
                {
                    "Red" or "red" => Color.Red,
                    "LightGreen1" or "lightgreen1" => Color.LightGreen1,
                    "Blue" or "blue" => Color.Blue,
                    "Grey" or "grey" => Color.Grey,
                    "LightGreen4" or "lightgreen4" => Color.LightGreen4,
                    "Orange" or "orange" => Color.Orange,
                    "LightGrey" or "lightgrey" => Color.LightGrey,
                    _ => throw new Exception("Invalid color for SetColor"),
                };
            }
        }
        public override byte[] ToControlSequence()
        {
            byte[] color = BitConverter.GetBytes((ushort)Color);
            return new byte[] { 0x00, 0x00, 0x03, 0x00, 0x02, 0x00, color[0], color[1] };
        }
        public override string ToControlString()
        {
            if (Color == Color.Reset)
            {
                return "</color>";
            }
            return $"<color={Color}>";
        }
    }
}
