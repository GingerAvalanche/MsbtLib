namespace MsbtLib.Controls
{
    enum PauseLength
    {
        Short,
        Long,
        Longer
    }
    internal class Pause : Control
    {
        public PauseLength Length { get; set; }
        public Pause(ushort length)
        {
            Length = (PauseLength)length;
        }
        public Pause(string str)
        {
            string parsed = str.Replace("<pause=", "").Replace("/>", "").Trim();
            Length = parsed switch
            {
                "Short" or "short" => PauseLength.Short,
                "Long" or "long" => PauseLength.Long,
                "Longer" or "longer" => PauseLength.Longer,
                _ => throw new Exception("Invalid pause length")
            };
        }
        public override byte[] ToControlSequence()
        {
            byte[] pause = BitConverter.GetBytes((ushort)Length);
            return new byte[] { 0x05, 0x00, pause[0], pause[1], 0x00, 0x00 };
        }
        public override string ToControlString()
        {
            return $"<pause={Length} />";
        }
    }
}
