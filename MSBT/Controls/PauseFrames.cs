using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    internal class PauseFrames : Control
    {
        public const ushort tag_group = 0x0001;
        public const ushort tag_type = 0x0000;
        private ushort param_size;
        private uint frames;
        public PauseFrames(List<ushort> parameters)
        {
            param_size = parameters[0];
            frames = parameters[1] == 0 ? parameters[2] : parameters[1]; // messy, but who wants to pause for more than >36.4083 minutes anyway?
        }
        public PauseFrames(string str)
        {
            Regex pattern = new(@"<pauseframes=(\d+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success || uint.Parse(m.Groups[1].ToString()) > 65535)
            {
                throw new Exception("Proper usage: <pauseframes=# /> where # is a number of frames between 0 and 65535. Valid examples: <pauseframes=30 /> or <pauseframes=60 />");
            }
            frames = uint.Parse(m.Groups[1].ToString());
            param_size = 2;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes(tag_type), 4);
            bytes.Merge(converter.GetBytes(param_size), 6);
            bytes.Merge(converter.GetBytes(frames), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<pauseframes={frames} />";
        }
    }
}
