using System.Text;

namespace MsbtLib
{
    static internal class Extensions
    {
        private static IEnumerable<IEnumerable<T>> ChunkCore<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            var c = 0;
            var batch = new List<T>();
            foreach (var item in enumerable)
            {
                batch.Add(item);
                if (++c % batchSize == 0)
                {
                    yield return batch;
                    batch = new List<T>();
                }
            }
            if (batch.Count != 0)
            {
                yield return batch;
            }
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
            return enumerable.ChunkCore(batchSize);
        }

        public static T[] Fill<T>(this T[] array, T val)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = val;
            }
            return array;
        }
        public static ushort DequeueUInt16(this Queue<byte> queue)
        {
            byte[] bytes = new byte[] { queue.Dequeue(), queue.Dequeue() };
            return BitConverter.ToUInt16(bytes);
        }
        public static void Merge<T>(this T[] arr, IEnumerable<T> other, int start_index)
        {
            if (arr.Length < (start_index + other.Count()))
            {
                Array.Resize(ref arr, start_index + other.Count());
            }
            int i = 0;
            foreach (T obj in other)
            {
                arr[start_index + i] = obj;
                i++;
            }
        }
        public static List<T> GetRange<T>(this List<T> list, Range range)
        {
            var (start, length) = range.GetOffsetAndLength(list.Count);
            return list.GetRange(start, length);
        }
        public static void Add<T>(this List<T> list, T add, int num)
        {
            for (int i = 0; i < num; i++)
            {
                list.Add(add);
            }
        }
    }
}
