using System.Text;
using System.Text.RegularExpressions;

namespace MsbtLib.Controls.AppTags
{
    internal class Variable : Control
    {
        public const string Tag = nameof(Variable);
        private readonly ushort _varType;
        private readonly ushort _paramSize;
        private readonly string _name;
        public Variable(ref VariableByteQueue queue)
        {
            _varType = queue.DequeueU16();
            _paramSize = queue.DequeueU16();
            if (_paramSize == 0)
            {
                _name = "";
                return;
            }
            (ushort nameSize, _name) = queue.DequeueCString();
            if (_paramSize != nameSize + 4) throw new InvalidDataException($"Variable param size mismatch");
            queue.DequeueU16(); // get rid of null that isn't part of the string for some reason
        }
        public Variable(string str)
        {
            Match m = new Regex($@"<{Tag}\stype=\w+(?:\((-?\d+)\))?\sname='((?:\w|\d)*)'\s/>").Match(str);
            if (!m.Success)
            {
                throw new ArgumentException($"Proper usage: <{Tag} type=X(#) name='?' /> where X is a type name, # is a number, and ? is a string of letters/numbers only, and may be empty. Valid example: <{Tag} type=Int(18) name=GiveItemNumber />");
            }
            _varType = ushort.Parse(m.Groups[1].ToString());
            _name = m.Groups[2].Value;
            _paramSize = (ushort)(_name.Length * 2 + 4);
        }
        public override byte[] ToControlSequence(EndiannessConverter converter)
        {
            byte[] bytes = new byte[_paramSize + 8];
            bytes.Merge(converter.GetBytes(ControlTag), 0);
            bytes.Merge(converter.GetBytes(AppTag.Group), 2);
            bytes.Merge(converter.GetBytes(_varType), 4);
            bytes.Merge(converter.GetBytes(_paramSize), 6);
            bytes.Merge(converter.GetCString(_name), 8);
            return bytes;
        }
        public override string ToControlString()
        {
            VariableType type = _varType < ControlHelpers.VariableTypes.Length ? ControlHelpers.VariableTypes[_varType] : VariableType.Unknown;
            return $"<{Tag} type={type}({_varType}) name='{_name}' />";
        }
    }
}
