namespace MsbtLib;

public class VariableByteQueue(List<byte> queue, Endianness endianness)
{
    private readonly Queue<byte> _queue = new(queue);
    public int Count => _queue.Count;
    
    public byte[] ToArray() => _queue.ToArray();
    
    public byte DequeueU8() => _queue.Dequeue();
    
    public ushort DequeueU16()
    {
        byte one = _queue.Dequeue();
        byte two = _queue.Dequeue();
        byte[] bytes = endianness switch
        {
            Endianness.Big => [two, one],
            Endianness.Little => [one, two],
            _ => throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null)
        };
        return BitConverter.ToUInt16(bytes, 0);
    }
    
    public uint DequeueU32()
    {
        byte one = _queue.Dequeue();
        byte two = _queue.Dequeue();
        byte three = _queue.Dequeue();
        byte four = _queue.Dequeue();
        byte[] bytes = endianness switch
        {
            Endianness.Big => [four, three, two, one],
            Endianness.Little => [one, two, three, four],
            _ => throw new ArgumentOutOfRangeException(nameof(endianness), endianness, null)
        };
        return BitConverter.ToUInt32(bytes, 0);
    }
}