using System.Text.RegularExpressions;

namespace MsbtLib.Controls
{
    enum PauseTime
    {
        Short,
        Long,
        Longer
    }
    internal class PauseLength : Control
    {
        public const ushort tag_group = 0x0005;
        public const ushort tag_type = 0x0000;
        private ushort param_size;
        private PauseTime time;
        public PauseLength(List<ushort> parameters)
        {
            param_size = parameters[0];
            time = (PauseTime)parameters[1];
        }
        public PauseLength(string str)
        {
            Regex pattern = new(@"<pauselength=(\w+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success || !Enum.TryParse(typeof(PauseTime), m.Groups[1].ToString(), out object? time))
            {
                throw new Exception("Proper usage: <pauselength=? /> where ? is a length: Short, Long, or Longer. Example: <pauselength=Long />");
            }
            if (time != null)
            {
                this.time = (PauseTime)time;
            }
            param_size = 0;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[param_size + 8];
            bytes.Merge(converter.GetBytes(control_tag), 0);
            bytes.Merge(converter.GetBytes(tag_group), 2);
            bytes.Merge(converter.GetBytes((ushort)time), 4); // yes, the parameter replaces the tag_type for PauseLengths. Thanks, Nintendo
            bytes.Merge(converter.GetBytes(param_size), 6);
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<pauselength={time} />";
        }
    }
}
