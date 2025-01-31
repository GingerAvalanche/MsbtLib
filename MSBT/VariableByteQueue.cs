using System.Buffers.Binary;
using System.Text;

namespace MsbtLib;

public ref struct VariableByteQueue(ref ReadOnlySpan<byte> buffer, Endianness endianness)
{
    private int _index = 0;
    private ReadOnlySpan<byte> _buffer = buffer;
    public int Count => _buffer[_index..].Length;
    
    public byte[] ToArray() => _buffer[_index..].ToArray();
    
    public byte DequeueU8() => _buffer[_index++];
    
    public ushort DequeueU16()
    {
        ushort value = endianness switch
        {
            Endianness.Big => BinaryPrimitives.ReadUInt16BigEndian(_buffer[_index..]),
            Endianness.Little => BinaryPrimitives.ReadUInt16LittleEndian(_buffer[_index..]),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null)
        };
        _index += 2;
        return value;
    }
    
    public uint DequeueU32()
    {
        uint value = endianness switch
        {
            Endianness.Big => BinaryPrimitives.ReadUInt32BigEndian(_buffer[_index..]),
            Endianness.Little => BinaryPrimitives.ReadUInt32LittleEndian(_buffer[_index..]),
            _ => throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null)
        };
        _index += 4;
        return value;
    }

    public (ushort, string) DequeueCString()
    {
        ushort length = DequeueU16();
        StringBuilder b = new();
        for (int i = 0; i < length / 2; ++i)
        {
            b.Append((char)DequeueU16());
        }
        return (length, b.ToString());
    }
}