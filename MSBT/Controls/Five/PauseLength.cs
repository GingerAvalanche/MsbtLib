using System.Text.RegularExpressions;

namespace MsbtLib.Controls.Five
{
    enum PauseTime
    {
        Short,
        Long,
        Longer
    }
    internal class PauseLength : Control
    {
        private readonly PauseTime _time;
        public PauseLength(ref VariableByteQueue queue)
        {
            _time = (PauseTime)queue.DequeueU16();
            if (queue.DequeueU16() != 0) throw new InvalidDataException("PauseLength parameter size not 0");
        }
        public PauseLength(string str)
        {
            Regex pattern = new(@"<pauselength=(\w+)\s/>");
            Match m = pattern.Match(str);
            if (!m.Success || !Enum.TryParse(typeof(PauseTime), m.Groups[1].ToString(), out object? time))
            {
                throw new Exception("Proper usage: <pauselength=? /> where ? is a length: Short, Long, or Longer. Example: <pauselength=Long />");
            }
            _time = (PauseTime)time;
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(FiveTag.Group), 2);
            bytes.Merge(converter.GetBytes((ushort)_time), 4); // yes, the parameter replaces the tag_type for PauseLengths. Thanks, Nintendo
            return bytes;
        }
        public override string ToControlString()
        {
            return $"<pauselength={_time} />";
        }
    }
}
