namespace MsbtLib
{
    internal static class Extensions
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
            ArgumentNullException.ThrowIfNull(enumerable);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);
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
        public static void Merge<T>(this T[] arr, IEnumerable<T> other, int startIndex)
        {
            T[] enumerable = other as T[] ?? other.ToArray();
            if (arr.Length < startIndex + enumerable.Length)
            {
                Array.Resize(ref arr, startIndex + enumerable.Length);
            }
            int i = 0;
            foreach (T obj in enumerable)
            {
                arr[startIndex + i] = obj;
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
