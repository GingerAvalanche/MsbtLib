using MsbtLib.Controls;

namespace MsbtLib
{
    abstract internal class Control
    {
        public static Control ParseControlBytes(ref Queue<byte> queue)
        {
            byte[] seqBytes = new byte[2];
            seqBytes[0] = queue.Dequeue();
            seqBytes[1] = queue.Dequeue();
            ushort seq1 = BitConverter.ToUInt16(seqBytes);
            switch (seq1)
            {
                case 0:
                    seqBytes[0] = queue.Dequeue();
                    seqBytes[1] = queue.Dequeue();
                    ushort seq2 = BitConverter.ToUInt16(seqBytes);
                    switch (seq2)
                    {
                        case 3:
                            seqBytes[0] = queue.Dequeue();
                            seqBytes[1] = queue.Dequeue();
                            seqBytes[0] = queue.Dequeue();
                            seqBytes[1] = queue.Dequeue();
                            return new SetColor(BitConverter.ToUInt16(seqBytes));
                        default:
                            throw new Exception($"Unknown control sequence type: {seq1} {seq2}");
                    }
                case 5:
                    seqBytes[0] = queue.Dequeue();
                    seqBytes[1] = queue.Dequeue();
                    queue.Dequeue();
                    queue.Dequeue();
                    return new Pause(BitConverter.ToUInt16(seqBytes));
                default:
                    throw new Exception($"Unknown control sequence type: {seq1}");
            }
        }
        public abstract byte[] ToControlSequence();
        public abstract string ToControlString();
    }
}
