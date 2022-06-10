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

        public static IEnumerable<byte> ToEndianness(this IEnumerable<byte> bytes, Endianness endianness)
        {
            return endianness switch
            {
                Endianness.Big => bytes.Reverse(),
                _ => bytes,
            };
        }

        public static string ToStringEncoding(this IEnumerable<byte> chars, UTFEncoding encoding)
        {
            return encoding switch
            {
                UTFEncoding.UTF16 => Encoding.Unicode.GetString(chars.ToArray()),
                _ => Encoding.UTF8.GetString(chars.ToArray()),
            };
        }

        public static T[] Fill<T>(this T[] array, T val)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = val;
            }
            return array;
        }
    }
}
